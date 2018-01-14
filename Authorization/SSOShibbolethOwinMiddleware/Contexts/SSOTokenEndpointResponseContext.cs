using System.Collections.Generic;
using Kernel.Authorisation.Contexts;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace SSOOwinMiddleware.Contexts
{

    public class SSOTokenEndpointResponseContext : EndpointContext<SSOAuthenticationOptions>, ITokenEndpointResponseContext
    {
        public SSOTokenEndpointResponseContext(IOwinContext context, SSOAuthenticationOptions options, string token, AuthenticationTicket ticket, IDictionary<string, object> relayState)
            : base(context, options)
        {
            this.Token = token;
            this.Ticket = ticket;
            this.RelayState = relayState;
        }
        public AuthenticationTicket Ticket { get; }
        public string Token { get; }
        public IDictionary<string, object> RelayState { get; }
    }
}