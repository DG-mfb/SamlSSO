using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal class RequestHelper
    {
        internal static Func<IEnumerable<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>> GetBuilders { get; set; }

        internal static Func<Type, bool> Condition = t => !t.IsAbstract && !t.IsInterface && typeof(ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>).IsAssignableFrom(t);
        internal static RequestAbstract BuildRequest(RequestContext requestContext)
        {
            if (RequestHelper.GetBuilders == null)
                throw new InvalidOperationException("GetBuilders factory not set");

            if (requestContext is AuthnRequestContext)
                return RequestHelper.BuildAuthnRequest((AuthnRequestContext)requestContext);

            if (requestContext is LogoutRequestContext)
                return RequestHelper.BuildLogoutRequest((LogoutRequestContext)requestContext);

            throw new NotSupportedException();
        }

        private static AuthnRequest BuildAuthnRequest(AuthnRequestContext requestContext)
        {
            if (RequestHelper.GetBuilders == null)
                throw new InvalidOperationException("GetBuilders factory not set");

            var requestConfig = requestContext.FederationPartyContext.GetAuthnRequestConfigurationFromContext(requestContext.RequestId);

            var request = new AuthnRequest
            {
                Destination = requestContext.Destination.AbsoluteUri,
            };

            if (requestContext.SupportedNameIdentifierFormats != null)
            {
                requestContext.SupportedNameIdentifierFormats.Aggregate(requestConfig.SupportedNameIdentifierFormats, (t, next) =>
                {
                    t.Add(next);
                    return t;
                });
            }

            var buiders = RequestHelper.GetBuilders();
            foreach (var b in buiders)
            {
                b.Build(request, requestConfig);
            }
            return request;
        }

        private static LogoutRequest BuildLogoutRequest(LogoutRequestContext requestContext)
        {
            return new LogoutRequest
            {
                Destination = requestContext.Destination.AbsoluteUri,
                Issuer = new NameId { Value = requestContext.FederationPartyContext.FederationPartyId },
                Reason = Reasons.User
            };
            throw new NotImplementedException();
        }
    }
}