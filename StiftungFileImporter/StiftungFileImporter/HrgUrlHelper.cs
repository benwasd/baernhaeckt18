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
            return "CHE-105.830.305";
        }

        private static string GetCantonForLegalSeatId(string registerOfficeId)
        {
            return "BE".ToLower();
        }
    }
}