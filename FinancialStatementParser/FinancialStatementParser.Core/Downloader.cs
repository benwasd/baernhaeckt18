using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinancialStatementParser.Core
{
    public class Downloader
    {
        public static async Task DownloadAsync(string urlToPdf, string targetFileName)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(urlToPdf);

                if (response.IsSuccessStatusCode)
                {
                    File.WriteAllBytes(targetFileName, await response.Content.ReadAsByteArrayAsync());
                }
                else
                {
                    Console.WriteLine("Error {0}, when accessing {1}", response.StatusCode, urlToPdf);
                }
            }
        }
    }
}
