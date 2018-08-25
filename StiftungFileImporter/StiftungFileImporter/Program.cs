using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp;
using AngleSharp.Parser.Html;
using CefSharp;
using CefSharp.OffScreen;
using Zefix;

namespace StiftungFileImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //var address = "https://be.chregister.ch/cr-portal/auszug/zefix.xhtml?uid=110389869&lang=de";
            //var address = "https://be.chregister.ch/cr-portal/auszug/auszug.xhtml?uid=CHE-105.830.305";

            var zefix = new ZefixSrv();
            Cef.Initialize();

            var companyNames = new [] {"\"Bibliomedia Schweiz - öffentliche Stiftung\" (BMS)"};

            foreach (var companyName in companyNames)
            {
                var companyInfo = zefix.FindByName(companyName);

                var address = HrgUrlHelper.GetQueryUrl(companyInfo);

                var browser = new ChromiumWebBrowser(address);
                browser.LoadingStateChanged += async (sender, e) =>
                {
                    if (e.IsLoading)
                    {
                        return;
                    }

                    await Task.Delay(5000);

                    var sourceVisitor = new TaskStringVisitor();
                    browser.GetMainFrame().GetSource(sourceVisitor);

                    var siteSource = await sourceVisitor.Task;
                    browser.Dispose();

                    // AngleSharp
                    var config = Configuration.Default.WithCss();
                    var parser = new HtmlParser(config);
                    var document = parser.Parse(siteSource);

                    foreach (var element in document.QuerySelector(".personen tbody").Children)
                    {
                        // unexpected row content or cancelled person
                        if (element.ChildElementCount != 6 || element.Children.Any(ce => ce.ClassList.Contains("strike"))
                            )
                        {
                            continue;
                        }

                        var person = element.Children[3].TextContent;
                        var function = element.Children[4].TextContent;
                        var permission = element.Children[5].TextContent;

                        if (function == "auditor")
                        {
                            // Could be a company
                        }
                    }
                };

                Console.ReadKey();
            }

            Cef.Shutdown();

            //var client = new HttpClient();
            //var response = client.GetAsync(address).Result;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    var body = response.Content.ReadAsStringAsync().Result;

            //}


            

        }
    }
}
