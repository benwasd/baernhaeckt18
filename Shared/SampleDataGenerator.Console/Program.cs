using Contracts;
using System;
using System.Collections.Generic;
using Nest;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new ConnectionSettings(new Uri("http://lb-agxv47bobldeo.westeurope.cloudapp.azure.com:9200/"));
            connection.BasicAuthentication("elastic", "***");
            connection.DefaultMappingFor<Stiftung>(s => s.IndexName("stiftungen"));

            var client = new ElasticClient(connection);

            foreach (var stiftung in GetSamples())
            {
                var indexResponse = client.IndexDocument(stiftung);
            }

            var searchResponse = client.Search<Stiftung>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.name)
                        .Query("Brot")
                     )
                )
            );

            var stiftungen = searchResponse.Documents;
        }

        private static IEnumerable<Stiftung> GetSamples()
        {
            yield return new Stiftung
            {
                handelsregisterUID = "CHE-112.566.591",
                name = "Cooperaxion, Stiftung für nachhaltige Entwicklung und interkulturellen Austausch",
                adresse = "Neue Winterthurerstrasse 26, 8304 Wallisellen",
                zweck = "Die Stiftung bezweckt die Unterstützung von Kindern in Not. Im Speziellen bezweckt die Stiftung die Erbringung von weltweiten Hilfeleistungen in Regionen ohne staatliche Basisversorgung zugunsten von Kindern in Not in Form der Bereitstellung von Unterkünften und Schulen, in welchen die Kinder betreut, erzogen und ausgebildet werden. Sie verfolgt ihren Zweck als gemeinnützige, nicht auf Gewinn ausgerichtete Institution und sie verfolgt weder Erwerbs-, Selbsthilfe- noch kommerzielle Zwecke. Sie kann auf eigene Rechnung oder treuhänderisch Finanzgeschäfte aller Art eingehen sowie Grundstücke erwerben, verwalten und veräussern. Das Stiftungsvermögen und allfällige Erträge daraus dürfen nur für die obenstehenden oder diesen verwandte Zwecke verwendet werden.",
                jahresbericht = "http://www.cooperaxion.org/_wp/wp-content/uploads/2012/05/Cooperaxion_Jahresbericht_2016_compr.pdf"
            };

            yield return new Stiftung
            {
                handelsregisterUID = "CHE-111.788.947",
                name = "Stiftung Brot für alle",
                adresse = "Bürenstrasse 12, 3007 Bern",
                zweck = "Die Stiftung fördert die kirchliche Entwicklungszusammenarbeit im In- und Ausland mit dem Ziel, Menschen auf dem Weg ihrer Befreiung aus Armut , Not und Hunger zu unterstützen, namentlich durch die Sammlung finanzieller Mittel für Entwicklungsprojekte, durch die Gewährleistung der Evaluation, Prüfung und Begleitung von Entwicklungsprojekten, durch Information der Öffentlichkeit und durch entwicklungspolitisches Engagement. Sie hat ausschliesslich gemeinnützigen Charakter und verfolgt keinerlei Erwerbszweck. Die Stiftung verfolgt zudem keine Selbsthilfezwecke.",
                jahresbericht = "https://brotfueralle.ch/content/uploads/2018/03/Bfa_JB17_Leporello-A5_D_GzD.pdf"
            };

            yield return new Stiftung
            {
                handelsregisterUID = "CHE-109.697.548",
                name = "Fondation Lascaux",
                adresse = "Zürichstrasse 42, 8306 Brüttisellen",
                zweck = "Die Stiftung bezweckt die Vermittlung, Erweiterung und Erforschung von bisher unbeachtet gebliebenen und neuen Erkenntnissen der Wirkung von Farben auf die geistig-spirituelle Entwicklung des Menschen, sowie deren Relevanz für eine neue, ganzheitliche Zusammenschau von Kultur, Wissenschaft und Wirtschaft. Grundlage sämtlicher Aktivitäten ist die Vorstellung eines einheitlichen, lebendigen Universums als Farb- und Klangschöpfung, basierend auf den zwölf Farben der kosmischen Ordnung. Es werden Vorträge und Seminare veranstaltet, die diese Farben in das Zentrum menschlicher Wahrnehmungs- und Bewusstseinsprozesse stellen. Darüber hinaus werden durch Dokumentation, Pflege und Erhaltung sowie öffentlich zugänglichen Präsentationen (Ausstellungen) bereits existierende Manifestationen in Kunst und Wissenschaft und ihren intermedialen Ausdrucksformen veranschaulicht. Die Stiftung verfolgt ihren Zweck als gemeinnützige, nicht auf Gewinn ausgerichtete Institution und sie verfolgt weder Erwerbs-, Selbsthilfe- noch kommerzielle Zwecke. Sie kann auf eigene Rechnung oder treuhänderisch Finanzgeschäfte aller Art eingehen sowie Grundstücke erwerben, verwalten und veräussern. Das Stiftungsvermögen und allfällige Erträge daraus dürfen nur für die obenstehenden oder diesen verwandte Zwecke verwendet werden.",
                jahresbericht = "http://www.cooperaxion.org/_wp/wp-content/uploads/2012/05/Cooperaxion_Jahresbericht_2016_compr.pdf"
            };
        }
    }
}
