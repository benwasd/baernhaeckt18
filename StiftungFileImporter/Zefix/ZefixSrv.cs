using System.Runtime.InteropServices;
using System.ServiceModel.Security;
using Zefix.ZefixReference;

namespace Zefix
{
    public class ZefixSrv
    {
        public CompanyInfo FindByName(string name)
        {
            var client = new ZefixReference.ZefixServicePortTypeClient();
            client.ClientCredentials.UserName.UserName = "Astrid.strahm@gs-edi.admin.ch";
            client.ClientCredentials.UserName.Password = "tKfxwRnc";

            //var response = client.GetByUidFull(new getByUidRequestType { uid = "110389869" });
            var response = client.SearchByName(new searchByNameRequest {name = name});
            var result = response.Item as Zefix.ZefixReference.shortResponseResult;
            if (result == null || result.companyInfo.Length <= 0)
            {
                return null;
            }

            var company = result.companyInfo[0];

            return new CompanyInfo
            {
                LegalSeatId = company.legalSeatId,
                Uid = company.uid,
                ChId = company.chid
            };
        }
    }
}