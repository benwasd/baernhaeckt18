using System.Runtime.InteropServices;
using System.ServiceModel.Security;
using Zefix.ZefixReference;

namespace Zefix
{
    public class ZefixSrv
    {
        public void FindByName(string name)
        {
            var client = new ZefixReference.ZefixServicePortTypeClient();
            client.ClientCredentials.UserName.UserName = "Name";
            client.ClientCredentials.UserName.Password = "PW";

            var response = client.SearchByName(new searchByNameRequest {name = name});
            
            //var response = client.GetByUidFull(new getByUidRequestType { uid = "110389869" });

        }
    }
}