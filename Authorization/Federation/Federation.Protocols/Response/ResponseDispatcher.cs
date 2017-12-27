using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
            await this.SendAsync((HttpPostResponseOutboundContext)context);
        }

        public async Task SendAsync(HttpPostResponseOutboundContext context)
        {
            using (var s = new StreamReader(@"D:\Dan\Software\Apira\Temp\MockResponse.xml"))
            {
                var response = s.ReadToEnd();
                var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(response));
                var relayState = new Dictionary<string, object>
                {
                    { "federationPartyId", "local" },
                };
                var relyingStateSerialised = await this._relayStateSerialiser.Serialize(relayState);
                context.Form.SetRequest(base64Encoded);
                context.Form.SetRelatState(relyingStateSerialised);
                context.Form.ActionURL = "http://localhost:60879/api/Account/SSOLogon";
                await context.DespatchDelegate(context.Form);
            }
        }
    }
}