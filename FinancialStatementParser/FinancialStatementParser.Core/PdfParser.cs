using System;
using System.Linq;
using Aspose.Pdf.Text;

namespace FinancialStatementParser.Core
{
    public class PdfParser
    {
        public static decimal FindTotalActiva(string path)
        {
            using (var document = new Aspose.Pdf.Document(path))
            {
                // Find correct page
                var x = new TextFragmentAbsorber("(?i)Total Aktiven", new TextSearchOptions(true));
                x.Visit(document);

                // Get all text from page
                var y = new TextAbsorber();
                y.Visit(x.TextFragments[1].Page);

                var lines = y.Text.Split('\n').Select(l => l.ToLowerInvariant()).ToArray();

                var totalActiva = lines.First(l => l.Contains("total aktiven"));
                totalActiva = totalActiva.Replace("total aktiven", "totalaktiven");
                var number = totalActiva.Split(new[] {' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)[1];

                var result = decimal.Parse(string.Join("", number.Where(char.IsDigit).ToArray()));

                if (lines.Any(l => l.Contains("in tchf")))
                {
                    result *= 1000;
                }

                if (lines.Any(l => l.Contains("in mchf") || totalActiva.Contains("mio")))
                {
                    result *= 1000000;
                }

                return result;
            }
        }
    }
}
