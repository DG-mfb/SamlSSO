﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Federation.Constants;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Request;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request
{
    internal class RequestHelper
    {
        internal static Func<IEnumerable<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>> GetAuthnRequestBuilders { get; set; }

        internal static Func<Type, bool> Condition = t => !t.IsAbstract && !t.IsInterface && typeof(ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>).IsAssignableFrom(t);
        internal static RequestAbstract BuildRequest(RequestContext requestContext)
        {
            if (RequestHelper.GetAuthnRequestBuilders == null)
                throw new InvalidOperationException("GetBuilders factory not set");

            if (requestContext is AuthnRequestContext)
                return RequestHelper.BuildAuthnRequest((AuthnRequestContext)requestContext);

            if (requestContext is LogoutRequestContext)
                return RequestHelper.BuildLogoutRequest((LogoutRequestContext)requestContext);

            throw new NotSupportedException();
        }

        private static AuthnRequest BuildAuthnRequest(AuthnRequestContext requestContext)
        {
            if (RequestHelper.GetAuthnRequestBuilders == null)
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

            var buiders = RequestHelper.GetAuthnRequestBuilders();
            foreach (var b in buiders)
            {
                b.Build(request, requestConfig);
            }
            return request;
        }

        private static LogoutRequest BuildLogoutRequest(LogoutRequestContext requestContext)
        {
            var configurtion = requestContext.FederationPartyContext.GetAuthnRequestConfigurationFromContext(requestContext.RequestId);
            return new LogoutRequest
            {
                Id = configurtion.RequestId,
                Destination = requestContext.Destination.AbsoluteUri,
                Issuer = new NameId { Value = configurtion.EntityId, Format = NameIdentifierFormats.Entity },
                Item = new NameId { Value = requestContext.SamlLogoutContext.NameId.Value, Format = requestContext.SamlLogoutContext.NameId.Format.AbsoluteUri },
                Reason = requestContext.SamlLogoutContext.Reason.AbsoluteUri,
                IssueInstant = DateTime.UtcNow,
                Version = configurtion.Version,
                SessionIndex = requestContext.SamlLogoutContext.SessionIndex
            };
        }
    }
}