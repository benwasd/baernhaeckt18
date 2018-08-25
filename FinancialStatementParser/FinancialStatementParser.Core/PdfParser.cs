using System;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace FinancialStatementParser.Core
{
    public class PdfParser
    {
        private const string TOTAL_ACTIVA_MARKER = "total aktiven"; // TODO: Make it work for multiple languages

        public static decimal FindTotalActiva(string path)
        {
            using (var document = new Aspose.Pdf.Document(path))
            {
                var activaPage = FindPage(document).TextFragments[1].Page;

                var totalActivaAbsober = new TextAbsorber();
                totalActivaAbsober.Visit(activaPage);

                var lines = totalActivaAbsober.Text.Split('\n').Select(l => l.ToLowerInvariant()).ToArray();

                var totalActiva = lines.First(l => l.Contains(TOTAL_ACTIVA_MARKER));
                totalActiva = totalActiva.Replace(TOTAL_ACTIVA_MARKER, TOTAL_ACTIVA_MARKER.Replace(" ", ""));
                var number = totalActiva.Split(new[] {' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)[1];

                var result = decimal.Parse(string.Join(string.Empty, number.Where(char.IsDigit).ToArray()));
                result = AdjustForCurrencyMultipliers(lines, result, totalActiva);

                return result;
            }
        }

        private static decimal AdjustForCurrencyMultipliers(string[] lines, decimal result, string totalActiva)
        {
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

        private static TextFragmentAbsorber FindPage(Document document)
        {
            var textFragmentAbsorber = new TextFragmentAbsorber("(?i)" + TOTAL_ACTIVA_MARKER, new TextSearchOptions(true));
            textFragmentAbsorber.Visit(document);

            return textFragmentAbsorber;
        }
    }
}
