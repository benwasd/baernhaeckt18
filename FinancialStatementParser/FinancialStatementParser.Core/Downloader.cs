using System;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace FinancialStatementParser.Core
{
    public class Downloader
    {
        public static void Download(string urlToPdf, string targetFileName)
        {
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += HandleDownloadComplete;

                var syncObject = new Object();
                lock (syncObject)
                {
                    client.DownloadFileAsync(new Uri(urlToPdf), targetFileName, syncObject);
                    //This would block the thread until download completes
                    Monitor.Wait(syncObject);
                }
            }
        }

        public static void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }
    }
}
