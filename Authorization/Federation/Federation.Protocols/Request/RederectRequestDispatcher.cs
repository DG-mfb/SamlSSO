using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;

namespace Federation.Protocols.Request
{
    internal class RederectRequestDispatcher : ISamlMessageDespatcher<HttpRedirectRequestContext>
    {
        private readonly IDependencyResolver _dependencyResolver;

        public RederectRequestDispatcher(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }

        public Task SendAsync(SamlOutboundContext context)
        {
            return this.SendAsync(context as HttpRedirectRequestContext);
        }

        public async Task SendAsync(HttpRedirectRequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var builders = this.GetBuilders();
            foreach (var b in builders.OrderBy(x => x.Order))
            {
                await b.Build(context.BindingContext);
            }
           
            await context.HanlerAction(context.BindingContext.GetDestinationUrl());
        }
        private IEnumerable<ISamlClauseBuilder> GetBuilders()
        {
            return this._dependencyResolver.ResolveAll<ISamlClauseBuilder>();
        }
    }
}