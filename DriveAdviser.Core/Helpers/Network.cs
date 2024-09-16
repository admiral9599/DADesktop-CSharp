using System;
using System.Net;
using System.Net.NetworkInformation;
using Serilog;

namespace DriveAdviser.Helpers
{
    public static class Network
    {
        public static bool HasInternetConnection
        {
            // There is no way you can reliably check if there is an internet connection, but we can come close
            get
            {
                var result = false;

                try
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                        using (var p = new Ping())
                        {
                            var pingReply = p.Send("8.8.8.8", 15000);
                            if (pingReply != null)
                                result =
                                    (pingReply.Status == IPStatus.Success) ||
                                    (p.Send("8.8.4.4", 15000)?.Status == IPStatus.Success) ||
                                    (p.Send("4.2.2.1", 15000)?.Status == IPStatus.Success);
                        }
                }
                catch
                {
                    // ignored
                }

                return result;
            }
        }

        public static string GetIpAddress()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(@"http://ipv4.icanhazip.com").Trim();

                }

            }
            catch (Exception e)
            {
                Log.Warning(e,"");
                return "0.0.0.0";
               

            }

          
        }
    }
}