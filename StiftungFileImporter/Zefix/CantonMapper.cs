using System.Collections.Generic;

namespace Zefix
{
    public static class CantonMapper
    {
        private static IDictionary<string, string> mapping = new Dictionary<string, string>
        {
            {"20", "ZH"},
            {"36", "BE"},
            {"100", "LU"},
            {"120", "UR"},
            {"130", "SZ"},
            {"140", "OW"},
            {"150", "NW"},
            {"160", "GL"},
            {"170", "ZG"},
            {"217", "FR"},
            {"241", "SO"},
            {"270", "BS"},
            {"280", "BL"},
            {"290", "SH"},
            {"300", "AR"},
            {"310", "AI"},
            {"320", "SG"},
            {"350", "GR"},
            {"400", "AG"},
            {"440", "TG"},
            {"501", "TI"},
            {"550", "VD"},
            {"600", "VS"},
            {"621", "VS"},
            {"626", "VS"},
            {"645", "NE"},
            {"660", "GE"},
            {"670", "JU"}
        };

        public static string MapByRegisterOfficeId(string registerOfficeId)
        {
            var canton = mapping.ContainsKey(registerOfficeId) ? mapping[registerOfficeId] : string.Empty;
            return canton.ToLower();
        }

        public static string CantonToCantonIso(string canton)
        {
            if (!string.IsNullOrWhiteSpace(canton))
            {
                return $"CH-{canton}";
            }

            return string.Empty;
        }
    }
}