using System;
using System.Linq;
using System.Net;
using System.Text;

namespace Kernel.Web
{
    public static class Utility
    {
        /// <summary>
        /// Checks if the url address is https
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsHttps(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;
            try
            {
                Uri uri = new Uri(address);
                return Utility.IsHttps(new Uri(address));
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the url address is https
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsHttps(Uri uri)
        {
            if (uri == (Uri)null)
                return false;
            return uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsLocalIpAddress(string host)
        {
            if (String.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("host");
            try
            {
                if (host.ToLower().StartsWith("localhost"))
                    return true;
                // get host IP addresses
                var hostIPs = Dns.GetHostAddresses(host);
                
                // get local IP addresses
                var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                
                foreach (var hostIP in hostIPs)
                {
                    if (IPAddress.IsLoopback(hostIP) || localIPs.Any(x => x.Equals(hostIP)))
                        return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static string UpperCaseUrlEncode(string value)
        {
            var result = new StringBuilder(value);
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] == '%')
                {
                    result[++i] = char.ToUpper(result[i]);
                    result[++i] = char.ToUpper(result[i]);
                }
            }

            return result.ToString();
        }
    }
}