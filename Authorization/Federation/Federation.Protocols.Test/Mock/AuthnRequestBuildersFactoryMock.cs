using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Protocols.Request;
using Federation.Protocols.Request.ClauseBuilders;
using Kernel.DependancyResolver;
using Kernel.Federation.FederationPartner;
using Kernel.Federation.Protocols;
using Kernel.Reflection;
using Shared.Federtion.Models;

namespace Federation.Protocols.Test.Mock
{
    internal class AuthnRequestBuildersFactoryMock
    {
        internal static Func<IEnumerable<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>> GetAuthnRequestBuildersFactory()
        {
            return () => ReflectionHelper.GetAllTypes(new[] { typeof(AutnRequestClauseBuilder).Assembly }, t => RequestHelper.Condition(t))
                .Select(x => (ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>)Activator.CreateInstance(x, (IDependencyResolver)null));
        }

        internal static Func<IEnumerable<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>> GetLogoutRequestBuildersFactory()
        {
            return () => Enumerable.Empty<ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>>();
            //return () => ReflectionHelper.GetAllTypes(new[] { typeof(AutnRequestClauseBuilder).Assembly }, t => RequestHelper.Condition(t))
            //    .Select(x => (ISamlRequestClauseBuilder<AuthnRequest, AuthnRequestConfiguration>)Activator.CreateInstance(x));
        }
    }
}