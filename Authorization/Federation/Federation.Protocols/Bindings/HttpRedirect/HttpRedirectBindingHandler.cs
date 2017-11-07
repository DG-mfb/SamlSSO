using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Federation.Protocols.Request.Handlers;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    internal class HttpRedirectBindingHandler : IBindingHandler
    {
        private readonly IDependencyResolver _dependencyResolver;
        public HttpRedirectBindingHandler(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }
        public async Task BuildRequest(HttpRedirectContext context)
        {
            var builders = this.GetBuilders();
            foreach(var b in builders.OrderBy(x => x.Order))
            {
                await b.Build(context);
            }
        }

        public async Task HandleOutbound(SamlOutboundContext context)
        {
            var builders = this.GetBuilders();
            foreach (var b in builders.OrderBy(x => x.Order))
            {
                await b.Build(context.BindingContext);
            }
            var httpRedirectContext = context as HttpRedirectRequestContext;
            await httpRedirectContext.HanlerAction(context.BindingContext.GetDestinationUrl());
        }

        public async Task HandleInbound(SamlInboundContext context)
        {
            var authnRequestHandler = this._dependencyResolver.Resolve<AuthnRequestHandler>();
            await authnRequestHandler.HandleRequest((HttpRedirectInboundContext)context);
        }

        private  IEnumerable<ISamlClauseBuilder> GetBuilders()
        {
            return this._dependencyResolver.ResolveAll<ISamlClauseBuilder>();
        }
    }
}