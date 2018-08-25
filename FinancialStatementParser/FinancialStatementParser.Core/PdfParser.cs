using System;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace FinancialStatementParser.Core
{
    public class PdfParser
    {
        private const string TOTAL_ACTIVA_MARKER = "total aktiven"; // TODO: Make it work for multiple languages
        private const string TOTAL_ACTIVA_MARKER_FALLBACK = "aktiven";

        public static decimal? FindTotalActiva(string path, int year)
        {
            decimal? result = null;

            using (var document = new Document(path))
            {
                var page = FindPage(document);

                if (page.TextFragments.Count > 0)
                {
                    var activaPage = page.TextFragments[1].Page;

                    var totalActivaAbsober = new TextAbsorber();
                    totalActivaAbsober.Visit(activaPage);

                   var lines = totalActivaAbsober.Text.Split('\n').Select(l => l.Replace("  ", " ").ToLowerInvariant()).ToArray();

                    if (!TryFindValue(lines, TOTAL_ACTIVA_MARKER, year, out result))
                    {
                        TryFindValue(lines, TOTAL_ACTIVA_MARKER_FALLBACK, year, out result);
                    }
                }
            }

            return result;
        }

        private static bool TryFindValue(string[] lines, string label, int year, out decimal? result)
        {
            try
            {
                var yearString = year.ToString();
                var totalActiva = lines.First(l => l.Contains(label) && !l.Contains(yearString.ToString()));
                var concatenatedLabel = label.Replace(" ", string.Empty);
                totalActiva = totalActiva.Replace(label, concatenatedLabel);

                //var split = totalActiva.Split(new[] {' ', ',', ';'}, StringSplitOptions.RemoveEmptyEntries);
                var split = totalActiva.Split(new[] { "  ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

                var number = split.SkipWhile(s => s != concatenatedLabel).Take(2).Last();

                result = decimal.Parse(string.Join(string.Empty, number.Where(char.IsDigit).ToArray()));
                result = AdjustForCurrencyMultipliers(lines, result.Value, totalActiva);

                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }            
        }

        private static decimal AdjustForCurrencyMultipliers(string[] lines, decimal result, string totalActiva)
        {
            if (lines.Any(l => l.Contains("in tchf") || l.Contains("1000 chf")))
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
            var textFragmentAbsorber = FindPageForText(document, TOTAL_ACTIVA_MARKER);
            if (textFragmentAbsorber.TextFragments.Count == 0)
            {
                textFragmentAbsorber = FindPageForText(document, TOTAL_ACTIVA_MARKER_FALLBACK);
            }

            return textFragmentAbsorber;
        }

        private static TextFragmentAbsorber FindPageForText(Document document, string text)
        {
            var textFragmentAbsorber = new TextFragmentAbsorber("(?i)" + text, new TextSearchOptions(true));
            textFragmentAbsorber.Visit(document);

            return textFragmentAbsorber;
        }
    }
}
