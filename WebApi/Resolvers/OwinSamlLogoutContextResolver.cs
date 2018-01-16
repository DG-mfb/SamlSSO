using System;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;
using Microsoft.Owin;

namespace WebApi.Resolvers
{
    internal class OwinSamlLogoutContextResolver : ISamlLogoutContextResolver<IOwinRequest>
    {
        private readonly IDependencyResolver _resolver;

        public OwinSamlLogoutContextResolver(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        public SamlLogoutContext ResolveLogoutContext(IOwinRequest request)
        {
           throw new NotImplementedException();
        }
    }
}