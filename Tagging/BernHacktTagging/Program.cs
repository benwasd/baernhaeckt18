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
                request.Headers.Add("Ocp-Apim-Subscription-Key", "2f3074698ebf464189c79c8ecd94b06b");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public static void Main(string[] args)
        {
            MainAsync(args);

            Console.ReadLine();
        }

        public static async void MainAsync(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //// Create a client.
            var client = ElasticSearchFactory.GetClient();

            TextAnalyticsAPI textAnalyticsClient = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials())
            {
                AzureRegion = AzureRegions.Westcentralus
            };

            var countResponse = client.Count<Stiftung>();
            var count = countResponse.Count;

            var searchSize = 20;
            var searchStart = 0; 

            do
            {
                await UpdateFoundationsTags(searchStart, searchSize, client, textAnalyticsClient);
                searchStart += searchSize;
            }
            while (searchStart < count);

            Console.WriteLine("You're finished. Great!");
        }

        private static async Task UpdateFoundationsTags(int searchStart, int searchSize, Nest.ElasticClient client, TextAnalyticsAPI textAnalyticsClient)
        {
            var searchResponse = client.Search<Stiftung>(s => s
                .From(searchStart)
                .Size(searchSize)
            );

            var stiftungen = searchResponse.Documents.Where(s => s.tags == null || s.tags.Length < 1).ToArray();
            if (stiftungen.Length >= 1)
            {

                var stiftungsZwecke = await GetPurposeDescriptionsAsMultiLanguageInput(textAnalyticsClient, stiftungen);

                if (stiftungsZwecke.Count < 1) return;
                KeyPhraseBatchResult result = await textAnalyticsClient.KeyPhrasesAsync(
                        new MultiLanguageBatchInput(stiftungsZwecke)
                    );

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
                    
                    var stiftung = new Stiftung();
                    stiftung.id = stiftungsId;
                    stiftung.tags = tags.ToArray();
                    stiftung.timestamp = DateTime.Now;

                    client.Update<Stiftung>(new Nest.DocumentPath<Stiftung>(stiftungsId), u => u.Doc(stiftung));
                }
                await Task.Delay(30000);
            }
        }
        
        private static async Task<List<MultiLanguageInput>> GetPurposeDescriptionsAsMultiLanguageInput(TextAnalyticsAPI client, IReadOnlyCollection<Stiftung> stiftungen)
        {
            List<MultiLanguageInput> purposeDescriptions = new List<MultiLanguageInput>();
            
            foreach (var stiftung in stiftungen.Where(s => !string.IsNullOrWhiteSpace(s.zweck)))
            {
                // extract language
                var languageResult = await client.DetectLanguageAsync(new BatchInput(
                    new List<Input>()
                    {
                          new Input("1", stiftung.zweck)
                    }));

                await Task.Delay(2000);

                purposeDescriptions.Add(new MultiLanguageInput(languageResult.Documents[0].DetectedLanguages[0].Iso6391Name, stiftung.id.ToString(), stiftung.zweck));
            }

            return purposeDescriptions;
        }
    }
}