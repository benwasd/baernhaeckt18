using System;
using System.Linq;
using System.Threading.Tasks;
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
            var zefix = new ZefixSrv();
            Cef.Initialize();
            var browser = new ChromiumWebBrowser();

            var companyNames = new[]
            {
                "\"Bibliomedia Schweiz - öffentliche Stiftung\" (BMS)",
                "Schweizerische Stiftung für Alpine Forschungen", "Pro Silva Helvetica"
            };
            var x = new System.Threading.ManualResetEvent(false);

            foreach (var companyName in companyNames)
            {
                var companyInfo = zefix.FindByName(companyName);
                if (companyInfo == null)
                {
                    Console.WriteLine($"Nothing found for '{companyName}'");
                    continue;
                }

                EventHandler<LoadingStateChangedEventArgs> loadedStateChanged = async (sender, e) =>
                {
                    if (e.IsLoading)
                    {
                        return;
                    }

                    Console.WriteLine($"Loading for company '{companyName}'");
                    await Task.Delay(5000);

                    var sourceVisitor = new TaskStringVisitor();
                    browser.GetMainFrame().GetSource(sourceVisitor);

                    var siteSource = await sourceVisitor.Task;

                    // AngleSharp
                    var config = Configuration.Default.WithCss();
                    var parser = new HtmlParser(config);
                    var document = parser.Parse(siteSource);

                    var tbody = document.QuerySelector(".personen tbody");
                    if (tbody != null)
                    {
                        foreach (var element in tbody.Children)
                        {
                            // unexpected row content or cancelled person
                            if (element.ChildElementCount != 6 ||
                                element.Children.Any(ce => ce.ClassList.Contains("strike")))
                            {
                                continue;
                            }

                            var person = element.Children[3].TextContent;
                            var function = element.Children[4].TextContent;
                            var permission = element.Children[5].TextContent;

                            Console.WriteLine($"person: {person}; function: {function}; permission: {permission}");

                            if (function == "auditor")
                            {
                                // Could be a company
                            }
                        }
                    }

                    x.Set();
                };

                browser.LoadingStateChanged += loadedStateChanged;

                var address = HrgUrlHelper.GetQueryUrl(companyInfo);
                browser.Load(address);

                x.WaitOne();
                x.Reset();

                browser.LoadingStateChanged -= loadedStateChanged;
            }

            Cef.Shutdown();
        }
    }
}