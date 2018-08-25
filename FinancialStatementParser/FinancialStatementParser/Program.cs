using System;
using System.IO;
using System.Linq;
using Contracts;
using FinancialStatementParser.Core;
using Nest;

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
                .Size(5000)
            );

            var stiftungen = searchResponse.Documents;

            var count = 0;

            foreach (var stiftung in stiftungen)
            {
                try
                {
                    count++;
                    Console.WriteLine($"Processing {count}: {stiftung.name}");
                    var result = ProcessFoundation(stiftung.name, stiftung.nameshort, 2017, stiftung.url);

                    if (result.Success)
                    {
                        var newStiftung = new Stiftung();
                        newStiftung.id = stiftung.id;
                        newStiftung.bilanzsumme = result.BalanceSheetTotal;
                        newStiftung.jahresbericht = result.FinancialStatementUrl.AbsoluteUri;

                        client.Update(new DocumentPath<Stiftung>(newStiftung.id), u => u.Doc(newStiftung));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error bei der Stiftung: {0} {1}", stiftung.name, ex);
                }
            }

            // Manual:
            // var rega = ProcessFoundation("Rega", 2017, "rega.ch");
            // var eichholz = ProcessFoundation("Eichholz", 2017, "stiftung-eichholz.ch");
            // var sieber = ProcessFoundation("Pfarrer Sieber", 2017, "swsieber.ch");
            // var hmsg = ProcessFoundation("HMSG", 2017, "hmsg.ch");

            Console.ReadKey();
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
            catch (Exception ex)
            {
                Console.WriteLine("ProcessFoundation: {0}", ex);
                return new FoundationResult { Success = false };
            }
        }

        private static void Initialize()
        {
            new Aspose.Pdf.License().SetLicense(AppDomain.CurrentDomain.BaseDirectory + @"\Aspose.pdf.lic");
        }
    }
}
