using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp;
using Zefix;

namespace StiftungFileImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //var address = "https://be.chregister.ch/cr-portal/auszug/zefix.xhtml?uid=110389869&lang=de";
            var address = "https://be.chregister.ch/cr-portal/auszug/auszug.xhtml?uid=CHE-105.830.305";

            

            //var client = new HttpClient();
            //var response = client.GetAsync(address).Result;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    var body = response.Content.ReadAsStringAsync().Result;

            //}


            // AngleSharp
            //var config = Configuration.Default.WithCookies().WithDefaultLoader(s =>
            //{
            //    s.IsNavigationEnabled = true;
            //    s.IsResourceLoadingEnabled = false;
            //});

            
        }
    }
}
