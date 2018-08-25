using System.Linq;
using Zefix;

namespace StiftungFileImporter
{
    public class HrgUrlHelper
    {
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
            return "BE".ToLower();
        }
    }
}