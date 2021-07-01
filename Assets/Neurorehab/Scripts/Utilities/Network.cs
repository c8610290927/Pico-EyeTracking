using System.Net;
using System.Net.Sockets;

namespace Neurorehab.Scripts.Utilities
{
    public static class Network
    {
        public static string GetLocalIP()
        {
            var localIp = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    localIp = ip.ToString();
            }

            return localIp;
        }
    }
}