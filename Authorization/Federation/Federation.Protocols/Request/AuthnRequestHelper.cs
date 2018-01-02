using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Federation.Protocols;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal class AuthnRequestHelper
    {
        internal static Func<IEnumerable<ISamlRequestClauseBuilder<AuthnRequest>>> GetBuilders { get; set; }

        internal static Func<Type, bool> Condition = t => !t.IsAbstract && !t.IsInterface && typeof(ISamlRequestClauseBuilder<AuthnRequest>).IsAssignableFrom(t);
        internal static AuthnRequest BuildAuthnRequest(AuthnRequestContext authnRequestContext)
        {
            if (AuthnRequestHelper.GetBuilders == null)
                throw new InvalidOperationException("GetBuilders factory not set");

            var requestConfig = authnRequestContext.FederationPartyContext.GetRequestConfigurationFromContext(authnRequestContext.RequestId);
            
            var request = new AuthnRequest
            {
                Destination = authnRequestContext.Destination.AbsoluteUri, 
            };

            if(authnRequestContext.SupportedNameIdentifierFormats != null)
            {
                authnRequestContext.SupportedNameIdentifierFormats.Aggregate(requestConfig.SupportedNameIdentifierFormats, (t, next) => 
                {
                    t.Add(next);
                    return t;
                });
            }

            var buiders = AuthnRequestHelper.GetBuilders();
            foreach(var b in buiders)
            {
                b.Build(request, requestConfig);
            }
            return request;
        }
    }
}