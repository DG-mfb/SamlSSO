﻿using System.Linq;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.ClauseBuilders
{
    internal class AudienceRestrictionClauseBuilder : AutnRequestClauseBuilder
    {
        public AudienceRestrictionClauseBuilder(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        {
        }

        protected override void BuildInternal(AuthnRequest request, AuthnRequestConfiguration configuration)
        {
            var audienceRestriction = new AudienceRestriction();
            configuration.AudienceRestriction.Aggregate(audienceRestriction, (a, next) => { a.Audience.Add(next); return a; });
            request.Conditions.Items.Add(audienceRestriction);
        }
    }
}