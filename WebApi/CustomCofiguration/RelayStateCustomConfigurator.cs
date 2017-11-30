using System;
using System.Linq;
using System.Collections.Generic;
using Kernel.Configuration;
using Microsoft.Owin;

namespace WebApi.CustomCofiguration
{
    public class RelayStateCustomConfigurator : ICustomConfigurator<IDictionary<string, object>>
    {
        public void Configure(IDictionary<string, object> configurable)
        {
            var origin = configurable["origin"];
            var uri = new Uri(origin.ToString());
            var queryStrings = QueryString.FromUriComponent(uri);
            var parsed = queryStrings.Value.Split(new[] { '&' })
                .ToDictionary(k => k.Substring(0, k.IndexOf('=')), v => v.Substring(v.IndexOf('=') + 1), StringComparer.OrdinalIgnoreCase);
            if (parsed.ContainsKey("returnUri"))
                configurable.Add("returnUri", parsed["returnUri"]);
        }
    }
}