using System;
using Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace FoundationCsvReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = ElasticSearchFactory.GetClient();

            var allLines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\StiftungsdatenUtf8.csv", Encoding.ASCII);

            Console.WriteLine("Total lines: " + allLines.Length);
            var count = 0;

            foreach (var line in allLines)
            {
                var values = line.Split(';');

                var stiftung = new Stiftung
                {
                    sourceId = values[0],
                    name = string.IsNullOrWhiteSpace(values[1]) ? string.IsNullOrWhiteSpace(values[2]) ? values[3] : values[2] : values[1],
                    nameshort = values[4],
                    adresse = values[5] + " " + values[6] + ", " + values[7] + " " + values[8],
                    url = values[9],
                    zweck = values[10]                    
                };

                var indexResponse = client.IndexDocument(stiftung);

                count++;
                Console.WriteLine("Processed line: " + count);
            }
        }
    }
}
