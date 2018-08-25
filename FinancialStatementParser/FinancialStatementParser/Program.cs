using System;
using System.IO;
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

            var client = ElasticSearchFactory.GetClient();

            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(20)
            );

            var stiftungen = searchResponse.Documents;

            foreach (var stiftung in stiftungen)
            {
                var result = ProcessFoundation(stiftung.name, stiftung.nameshort, 2017, stiftung.url);

                if (result.Success)
                {
                    stiftung.bilanzsumme = result.BalanceSheetTotal;
                    stiftung.jahresbericht = result.FinancialStatementUrl.AbsoluteUri;

                    //client.Update();
                }
            }

            // Manual:
            // var rega = ProcessFoundation("Rega", 2017, "rega.ch");
            // var eichholz = ProcessFoundation("Eichholz", 2017, "stiftung-eichholz.ch");
            // var sieber = ProcessFoundation("Pfarrer Sieber", 2017, "swsieber.ch");
            // var hmsg = ProcessFoundation("HMSG", 2017, "hmsg.ch");
        }

        private static FoundationResult ProcessFoundation(string foundation, string shortName, int year, string host = null)
        {
            var downloadUri = AppDomain.CurrentDomain.BaseDirectory + @"\Data\" + $"{shortName}_{year}.pdf";

            var result = Crawler.FindFinancialStatement(foundation, year, host).ToArray();

            var jahresRechnungUrl = result.FirstOrDefault(r => r.AbsoluteUri.Contains("rechnung")) ?? result.FirstOrDefault();

            if (jahresRechnungUrl == null)
            {
                return new FoundationResult { Success = false };
            }

            if (!File.Exists(downloadUri)) // Don't download the file if we already have it
            {
                Downloader.Download(jahresRechnungUrl.AbsoluteUri, downloadUri);
            }

            try
            {
                var balanceSheetTotal = PdfParser.FindTotalActiva(downloadUri, year).GetValueOrDefault();

                return new FoundationResult
                {
                    FinancialStatementUrl = jahresRechnungUrl,
                    BalanceSheetTotal = balanceSheetTotal,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new FoundationResult { Success = false };
            }
        }

        private static void Initialize()
        {
            new Aspose.Pdf.License().SetLicense(AppDomain.CurrentDomain.BaseDirectory + @"\Aspose.pdf.lic");
        }
    }
}
