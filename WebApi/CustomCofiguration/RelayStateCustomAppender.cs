using System;
using System.Threading.Tasks;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using SSOOwinMiddleware;

namespace WebApi.CustomCofiguration
{
    public class RelayStateCustomAppender : IRelayStateAppender
    {
        public Task BuildRelayState(RequestContext authnRequestContext)
        {
            var owinRequest = authnRequestContext as OwinAuthnRequestContext;
            if (owinRequest != null)
            {
                var query = owinRequest.Context.Request.Query;
                var returnUri = query.Get(RelayStateContstants.RedirectUrl);
                if (!String.IsNullOrWhiteSpace(returnUri))
                    owinRequest.RelyingState.Add(RelayStateContstants.RedirectUrl, returnUri);
            }
            return Task.CompletedTask;
        }
    }
}