using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;
using CefSharp;
using CefSharp.OffScreen;
using Contracts;
using Nest;
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
            var browserManualResetEvent = new ManualResetEvent(false);
            var elasticClient = ElasticSearchFactory.GetClient();

            var stiftungen = elasticClient.Search<Stiftung>(s => s.From(0));

            foreach (var stiftung in stiftungen.Documents)
            {
                var companyName = stiftung.name;
                var companyInfo = zefix.FindByName(companyName);
                if (companyInfo == null)
                {
                    Console.WriteLine($"Nothing found for '{companyName}'");

                    Thread.Sleep(3000);
                    continue;
                }

                var newStiftung = new Stiftung
                {
                    id = stiftung.id,
                    handelsregisterUID = companyInfo.Uid,
                    handelsregisterCHNR = companyInfo.ChId,
                    handelsregisterAmt = companyInfo.RegisterOfficeId,
                    kanton = companyInfo.CantonIso
                };

                var hadDelay = false;

                EventHandler<LoadingStateChangedEventArgs> loadedStateChanged = async (sender, e) =>
                {
                    if (e.IsLoading)
                    {
                        return;
                    }

                    Console.WriteLine($"Loading for company '{companyName}'");
                    await Task.Delay(5000);

                    hadDelay = true;

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
                        var members = new List<Stiftungsratsmitglied>();
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

                        newStiftung.stiftungsratsmitglieder = members.ToArray();
                    }

                    browserManualResetEvent.Set();
                };

                browser.LoadingStateChanged += loadedStateChanged;

                var address = HrgUrlHelper.GetQueryUrl(companyInfo);
                browser.Load(address);

                browserManualResetEvent.WaitOne();
                browserManualResetEvent.Reset();

                browser.LoadingStateChanged -= loadedStateChanged;

                if (!hadDelay)
                {
                    Thread.Sleep(5000);
                }

                newStiftung.timestamp = DateTime.Now;
                elasticClient.Update(new DocumentPath<Stiftung>(stiftung.id), u => u.Doc(newStiftung));
            }

            Cef.Shutdown();

            Console.WriteLine();
            Console.WriteLine("FINISHED :-)");
            Console.ReadKey();
        }
    }
}