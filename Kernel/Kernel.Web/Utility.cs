﻿using System;
using System.Net;

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
            try
            { // get host IP addresses
                var hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (var hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP))
                        return true;

                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP))
                            return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}