using System;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class CommomAttributesBuilder : ClauseBuilder
    {
        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            request.IsPassive = configuration.IsPassive;
            request.ForceAuthn = configuration.ForceAuthn;
            request.Version = configuration.Version;
            request.IssueInstant = DateTime.UtcNow;
        }
    }
}