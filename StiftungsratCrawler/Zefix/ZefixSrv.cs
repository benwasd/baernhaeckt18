using System;
using System.ServiceModel;
using System.Threading;
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

            var companyName = GetNameForSearch(name);
            shortResponse response = null;
            try
            {
                response = client.SearchByName(new searchByNameRequest { name = companyName });
            }
            catch (ProtocolException e)
            {
                Console.WriteLine($"Exception occured during search for '{companyName}'");
                Console.WriteLine(e);
                Thread.Sleep(5000);

                return null;
            }
            
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
                ChId = company.chid,
                Canton = CantonMapper.MapByRegisterOfficeId(company.registerOfficeId)
            };
        }

        private static string GetNameForSearch(string stiftungName)
        {
            var name = stiftungName;
            if (name == null)
            {
                return string.Empty;
            }

            // Is an escape pattern in the export -> remove for search
            name = name.Replace("<(>&<)>", "*");

            // Sometimes, the translated name(s) are appendend in the german name field, surrounded by brackets.
            var bracketPosition = name.IndexOf('(');
            if (bracketPosition > 1)
            {
                name = name.Substring(0, bracketPosition);
            }

            return name.Trim();
        }
    }
}