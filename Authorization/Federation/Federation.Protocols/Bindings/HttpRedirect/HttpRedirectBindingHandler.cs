using System;
using System.Threading.Tasks;
using Federation.Protocols.Factories;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpRedirect
{
    internal class HttpRedirectBindingHandler : IBindingHandler
    {
        private readonly IDependencyResolver _dependencyResolver;
        public HttpRedirectBindingHandler(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }
        
        public async Task HandleOutbound(SamlOutboundContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var despatcher = this._dependencyResolver.Resolve(typeof(ISamlMessageDespatcher<>).MakeGenericType(context.GetType())) as ISamlMessageDespatcher;
            await despatcher.SendAsync(context);
        }

        public async Task HandleInbound(SamlInboundContext context)
        {
            var del = InboundHandleFactory.GetHandleDelegate(context.GetType());
            var handler = this._dependencyResolver.Resolve(typeof(IInboundHandler<>).MakeGenericType(context.GetType()));
            
            await del(handler, context);
        }
    }
}