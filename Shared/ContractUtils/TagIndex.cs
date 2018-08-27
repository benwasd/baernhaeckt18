using System.Linq;
using Contracts;

namespace ContractUtils
{
    public class TagIndex
    {
        public void FillTagsByStiftungen()
        {
            var client = ElasticSearchFactory.GetClient();

            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(10000)
            );

            var stiftungen = searchResponse.Documents;

            var tags = stiftungen
                .Where(s => s.tags != null)
                .SelectMany(
                    s => s.tags
                        .Where(FilterTag)
                        .Select(t => new Tag { kanton = s.kanton, name = t, stiftungId = s.id, stiftungName = s.name })
                )
                .Where(t => t != null);

            foreach (var tag in tags)
            {
                client.IndexDocument(tag);
            }
        }

        private static bool FilterTag(string tag)
        {
            var lowerTag = tag.ToLowerInvariant();

            var toRemove = lowerTag.Contains("stiftung")
                        || lowerTag.Contains("Stiftung")
                        || lowerTag.Contains("Unterstützung")
                        || lowerTag.Contains("Förderung")
                        || lowerTag.Contains("Schweiz")
                        || lowerTag.Contains("but lucratif")
                        || lowerTag.Contains("Zweck der Stiftung")
                        || lowerTag.Contains("fondation")
                        || lowerTag.Contains("but")
                        || lowerTag.Contains("Institutionen")
                        || lowerTag.Contains("Rahmen")
                        || lowerTag.Contains("Suisse")
                        || lowerTag.Contains("Gewinn")
                        || lowerTag.Contains("Fondation")
                        || lowerTag.Contains("Organisationen")
                        || lowerTag.Contains("Projekten")
                        || lowerTag.Contains("Projekte")
                        || lowerTag.Contains("éducation")
                        || lowerTag.Contains("ZGB")
                        || lowerTag.Contains("Projekten")
                        || lowerTag.Contains("Projekte")
                        || lowerTag.Contains("éducation")
                        || lowerTag.Contains("ZGB")
                        || lowerTag.Contains("Erwerbszweck")
                        || lowerTag.Contains("particulier")
                        || lowerTag.Contains("rement")
                        || lowerTag.Contains("Personen")
                        || lowerTag.Contains("caract")
                        || lowerTag.Contains("Sinne")
                        || lowerTag.Contains("Stiftungsrat")
                        || lowerTag.Contains("Stifter")
                        || lowerTag.Contains("Zwecksetzung")
                        || lowerTag.Contains("keinerlei Erwerbszweck")
                        || lowerTag.Contains("monde")
                        || lowerTag.Contains("Stifter")
                        || lowerTag.Contains("Stiftungszwecks")
                        || lowerTag.Contains("recherche")
                        || lowerTag.Contains("intér")
                        || lowerTag.Contains("Durchführung");

            return toRemove == false;
        }
    }
}
