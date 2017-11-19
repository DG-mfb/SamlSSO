using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.MetaData.Configuration.EndPoint;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;

namespace Federation.Protocols.Response
{
    internal class ResponseDispatcher : ISamlMessageDespatcher<HttpPostResponseOutboundContext>
    {
        private readonly ResponseBuilder _responseBuilder;
        private IRelayStateSerialiser _relayStateSerialiser;
        public ResponseDispatcher(ResponseBuilder responseBuilder, IRelayStateSerialiser relayStateSerialiser)
        {
            this._relayStateSerialiser = relayStateSerialiser;
            this._responseBuilder = responseBuilder;
        }
        public async Task SendAsync(SamlOutboundContext context)
        {
            var httpClient = new HttpClient();
            using (var s = new StreamReader(@"D:\Dan\Software\Apira\Temp\MockResponse.xml"))
            {
                var response = s.ReadToEnd();
                var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(response));
                var relayState = new Dictionary<string, object>
                {
                    { "federationPartyId", "local" },
                    {"assertionConsumerServices", new List<IndexedEndPointConfiguration>{ new IndexedEndPointConfiguration { Location = new Uri("http://localhost:60879/api/Account/SSOLogon") } } }
                };
                var relyingStateSerialised = await this._relayStateSerialiser.Serialize(relayState);
                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("SAMLResponse", base64Encoded),
                    new KeyValuePair<string, string>("RelayState", relyingStateSerialised)
                });
                await httpClient.PostAsync("http://localhost:60879/api/Account/SSOLogon", content);
            }
            
            //throw new NotImplementedException();
        }

        public Task SendAsync(HttpPostResponseOutboundContext context)
        {
            throw new NotImplementedException();
        }
    }
}