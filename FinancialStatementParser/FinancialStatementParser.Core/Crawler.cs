using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Text.RegularExpressions;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;

namespace FinancialStatementParser.Core
{
    public class Crawler
    {
        public static IEnumerable<Uri> FindFinancialStatement(string foundation, int year, string host= null)
        {
            var result = new List<Uri>();

            using (var webClient = new WebClient())
            {
                var searchUri = host == null ? $"https://www.google.ch/search?q={foundation}+pdf+jahresrechnung+{year}" : $"https://www.google.ch/search?q=site:{host}+pdf+jahresrechnung+{year}";
                var pageString = webClient.DownloadString(searchUri);

                var uris =
                    new Regex(@"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?")
                        .Matches(pageString).OfType<Match>().Select(m => m.Value).ToArray();

                foreach (var match in uris.Where(m => !m.Contains("webcache")).Where(m => m.Contains(".pdf")))
                {
                    result.Add(new Uri(match.Replace("&amp", string.Empty)));
                }
            }

            return result;
        }

        /// <summary>
        /// Limited to 100 requests per day, replaces with html parser
        /// </summary>
        public static IEnumerable<Result> FindFinancialStatementOld(string foundation, int year, string host = null)
        {
            foundation = foundation.ToLowerInvariant();

            var apiKey = "AIzaSyD8pa5nQ7OnS9gY--JK-FpH9EpJkFZD16s";
            var cx = "012808976328688575091:hm9sbvtozky";
            var query = foundation + " Jahresrechnung " + year;

            var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
            var listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = cx;
            listRequest.FileType = "pdf";
            listRequest.SiteSearch = host;

            Console.WriteLine("Start...");
            IList<Result> paging = new List<Result>();
            var count = 0;

            var result = new List<Result>();
            while (paging != null)
            {
                Console.WriteLine($"Page {count}");
                listRequest.Start = count * 10 + 1;
                paging = listRequest.Execute().Items;
                if (paging != null)
                {
                    foreach (var item in paging)
                    {
                        //if (item.Link.ToLowerInvariant().Contains("jahresrechnung") && item.Link.Contains(foundation) && item.Link.EndsWith(".pdf"))
                        result.Add(item);
                    }
                }

                count++;
            }

            return result;
        }
    }
}
