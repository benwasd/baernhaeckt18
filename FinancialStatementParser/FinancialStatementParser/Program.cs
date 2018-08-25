using System;
using System.Linq;
using Contracts;
using FinancialStatementParser.Core;

namespace FinancialStatementParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();

            /*var client = ElasticSearchFactory.GetClient();

            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(10)
            );

            var stiftungen = searchResponse.Documents;

            foreach (var stiftung in stiftungen)
            {
                // stiftung.url;
                // stiftung.name;

                stiftung.bilanzsumme = 0;
                stiftung.jahresbericht = "https://stiftung.com/2017/jahresrechnung.pdf";
            }*/

            // Manual:
            var rega = ProcessFoundation("Rega", 2017, "rega.ch");
            // var eichholz = ProcessFoundation("Eichholz", 2017, "stiftung-eichholz.ch");
            // var sieber = ProcessFoundation("Pfarrer Sieber", 2017, "swsieber.ch");
            // var hmsg = ProcessFoundation("HMSG", 2017, "hmsg.ch");
        }

        private static decimal? ProcessFoundation(string foundation, int year, string host = null)
        {
            var result = Crawler.FindFinancialStatement(foundation, year, host).ToArray();

            var jahresRechnung = result.FirstOrDefault(r => r.AbsoluteUri.Contains("rechnung"));
            if (jahresRechnung == null)
            {
                jahresRechnung = result.FirstOrDefault();
            }

            if (jahresRechnung != null)
            {
                Downloader.Download(jahresRechnung.AbsoluteUri, AppDomain.CurrentDomain.BaseDirectory + @"\Data",
                    $"{foundation}_{year}.pdf");

                return PdfParser.FindTotalActiva(AppDomain.CurrentDomain.BaseDirectory + @"\Data\" + $"{foundation}_{year}.pdf");
            }

            return null;
        }

        private static void Initialize()
        {
            new Aspose.Pdf.License().SetLicense(AppDomain.CurrentDomain.BaseDirectory + @"\Aspose.pdf.lic");
        }
    }
}
