using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;
using CefSharp;
using CefSharp.OffScreen;
using Contracts;
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
            var x = new System.Threading.ManualResetEvent(false);
            var elasticClient = ElasticSearchFactory.GetClient();

            //var companyNames = new[]
            //{
            //    "\"Bibliomedia Schweiz - öffentliche Stiftung\" (BMS)",
            //    "Schweizerische Stiftung für Alpine Forschungen", "Pro Silva Helvetica"
            //};

            var stiftungen = elasticClient.Search<Stiftung>();

            //foreach (var companyName in companyNames)
            foreach (var stiftung in stiftungen.Documents)
            {
                var companyName = stiftung.name;
                var companyInfo = zefix.FindByName(companyName);
                if (companyInfo == null)
                {
                    Console.WriteLine($"Nothing found for '{companyName}'");
                    continue;
                }

                stiftung.handelsregisterUID = companyInfo.Uid;
                stiftung.handelsregisterCHNR = companyInfo.ChId;
                stiftung.handelsregisterAmt = companyInfo.RegisterOfficeId;

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
                        var members =
                            new List<Stiftungsratsmitglied>(stiftung.stiftungsratsmitglieder ??
                                                            Enumerable.Empty<Stiftungsratsmitglied>());
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

                            // Could be a company -> exclude
                            if (function != "auditor")
                            {
                                var member = new Stiftungsratsmitglied { name = person, funktion = function, berechtigung = permission };
                                members.Add(member);
                            }

                        }

                        stiftung.stiftungsratsmitglieder = members.ToArray();
                    }

                    x.Set();
                };

                browser.LoadingStateChanged += loadedStateChanged;

                var address = HrgUrlHelper.GetQueryUrl(companyInfo);
                browser.Load(address);

                x.WaitOne();
                x.Reset();

                browser.LoadingStateChanged -= loadedStateChanged;

                stiftung.timestamp = DateTime.Now;
                elasticClient.IndexDocument(stiftung);
            }

            Cef.Shutdown();
        }
    }
}