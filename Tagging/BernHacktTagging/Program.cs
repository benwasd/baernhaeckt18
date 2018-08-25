using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using System.Linq;

namespace BernHacktTagging
{
    public class Program
    {
        /// <summary>
        /// Container for subscription credentials. Make sure to enter your valid key.
        /// </summary>
        public class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", "255add7f1e504d3aac98e74f759acd1a");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public static void Main(string[] args)
        {
            //// Create a client.
            var client = ElasticSearchFactory.GetClient();

            TextAnalyticsAPI textAnalyticsClient = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials())
            {
                AzureRegion = AzureRegions.Westcentralus // example 
            };

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // so oft wiederholen wie nötig
            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(100)
            );

            var stiftungen = searchResponse.Documents;
            var stiftungsZwecke = GetPurposeDescriptionsAsMultiLanguageInput(textAnalyticsClient, stiftungen);

            KeyPhraseBatchResult result = textAnalyticsClient.KeyPhrasesAsync(
                    new MultiLanguageBatchInput(stiftungsZwecke)
                ).Result;

            // Printing key phrases and writing phrases to stiftung.
            foreach (var document in result.Documents)
            {
                Console.WriteLine("Document ID: {0} ", document.Id);

                Console.WriteLine("\t Key phrases:");

                var tags = new List<string>();
                Guid stiftungsId = new Guid(document.Id);

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine("\t\t" + keyphrase);
                    tags.Add(keyphrase);
                }

                var stiftung = stiftungen.FirstOrDefault(s => s.id.Equals(stiftungsId));
                stiftung.tags = tags.ToArray();
                stiftung.timestamp = DateTime.Now;

                client.IndexDocument(stiftung);
            }

            Console.ReadLine();
        }
        
        private static List<MultiLanguageInput> GetPurposeDescriptionsAsMultiLanguageInput(TextAnalyticsAPI client, IReadOnlyCollection<Stiftung> stiftungen)
        {
            List<MultiLanguageInput> purposeDescriptions = new List<MultiLanguageInput>();
            
            foreach (var stiftung in stiftungen)
            {
                // extract language
                var languageResult = client.DetectLanguageAsync(new BatchInput(
                    new List<Input>()
                    {
                          new Input("1", stiftung.zweck)
                    })).Result;

                purposeDescriptions.Add(new MultiLanguageInput(languageResult.Documents[0].DetectedLanguages[0].Iso6391Name, stiftung.id.ToString(), stiftung.zweck));
            }

            return purposeDescriptions;
        }
    }
}