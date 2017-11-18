using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Logging;
using Shared.Federtion.Forms;

namespace Federation.Protocols.Request
{
    internal class RequestDispatcher
    {
        private readonly IAuthnRequestSerialiser _serialiser;
        private readonly ILogProvider _logProvider;
        private IRelayStateSerialiser _relayStateSerialiser;

        public RequestDispatcher(IAuthnRequestSerialiser serialiser, IRelayStateSerialiser relayStateSerialiser, ILogProvider logProvider)
        {
            this._serialiser = serialiser;
            this._logProvider = logProvider;
            this._relayStateSerialiser = relayStateSerialiser;
        }
        public async Task SendAsync(SamlOutboundContext context)
        {
            var request = AuthnRequestHelper.BuildAuthnRequest(context.BindingContext.AuthnRequestContext);
            var httpClient = new HttpClient();
            var serialised = this._serialiser.Serialize(request);

            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialised));
           
            var relyingStateSerialised = await this._relayStateSerialiser.Serialize(context.BindingContext.RelayState);
            var form = new SAMLForm
            {
                ActionURL = context.BindingContext.DestinationUri.AbsoluteUri
            };
            form.AddHiddenControl("SAMLRequest", base64Encoded);
            form.AddHiddenControl("RelayState", relyingStateSerialised);
            var samlForm = form.ToString();
            await ((HttpPosttRequestContext)context).HanlerAction(samlForm);
        }
    }
}