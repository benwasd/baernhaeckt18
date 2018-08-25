using Zefix.ZefixReference;

namespace Zefix
{
    public class ZefixSrv
    {
        public CompanyInfo FindByName(string name)
        {
            var client = new ZefixServicePortTypeClient();
            client.ClientCredentials.UserName.UserName = "UserName";
            client.ClientCredentials.UserName.Password = "Password";

            var response = client.SearchByName(new searchByNameRequest {name = name});
            var result = response.Item as shortResponseResult;
            if (result?.companyInfo == null || result.companyInfo.Length <= 0)
            {
                return null;
            }

            var company = result.companyInfo[0];

            return new CompanyInfo
            {
                LegalSeatId = company.legalSeatId,
                RegisterOfficeId = company.registerOfficeId,
                Uid = company.uid,
                ChId = company.chid
            };
        }
    }
}