using System.Collections.Generic;
using System.Linq;
using Zefix;

namespace StiftungFileImporter
{
    public class HrgUrlHelper
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
        public static string GetQueryUrl(CompanyInfo companyInfo)
        {
            var canton = GetCantonForLegalSeatId(companyInfo.RegisterOfficeId);
            var uid = FormatUid(companyInfo.Uid);

            return $"https://{canton}.chregister.ch/cr-portal/auszug/auszug.xhtml?uid={uid}";
        }

        private static string FormatUid(string uid)
        {
            var part1 = string.Concat(uid.Skip(0).Take(3));
            var part2 = string.Concat(uid.Skip(3).Take(3));
            var part3 = string.Concat(uid.Skip(6).Take(3));

            var s = $"CHE-{part1}.{part2}.{part3}";

            return s;
        }

        private static string GetCantonForLegalSeatId(string registerOfficeId)
        {
            var canton = mapping.ContainsKey(registerOfficeId) ? mapping[registerOfficeId] : "be";
            return canton.ToLower();
        }
    }
}