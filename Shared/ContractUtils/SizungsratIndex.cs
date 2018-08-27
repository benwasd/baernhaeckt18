using System.Linq;
using Contracts;

namespace ContractUtils
{
    public class SizungsratIndex
    {
        public void FillSizungsraeteByStiftungen()
        {
            var client = ElasticSearchFactory.GetClient();

            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(10000)
            );

            var stiftungen = searchResponse.Documents;

            var stiftungsraete = stiftungen
                .Where(s => s.stiftungsratsmitglieder != null)
                .SelectMany(
                    s => s.stiftungsratsmitglieder
                        .Where(sr => sr?.name != null)
                        .Select(t => new Stiftungsrat { kanton = s.kanton, name = GetOnlyName(t.name), nameFull = t.name, stiftungId = s.id, stiftungName = s.name })
                )
                .Where(t => t != null);

            foreach (var stiftungsrat in stiftungsraete)
            {
                client.IndexDocument(stiftungsrat);
            }
        }

        private string GetOnlyName(string nameFull)
        {
            return string.Join(',', nameFull.Split(',').Take(2));
        }
    }
}
