using System;
using System.Threading.Tasks;
using Federation.Protocols.Factories;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpPost
{
    internal class HttpPostBindingHandler : IBindingHandler
    {
        private readonly IDependencyResolver _dependencyResolver;
        public HttpPostBindingHandler(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }
        
        public async Task HandleOutbound(SamlOutboundContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var dispatcher = this._dependencyResolver.Resolve(typeof(ISamlMessageDespatcher<>).MakeGenericType(context.GetType())) as ISamlMessageDespatcher;
            await dispatcher.SendAsync(context);
        }
        
        public async Task HandleInbound(SamlInboundContext context)
        {
            var del = InboundHandleFactory.GetHandleDelegate(context.GetType());
            var responseHandler = this._dependencyResolver.Resolve(typeof(IInboundHandler<>).MakeGenericType(context.GetType()));
            await del(responseHandler, context);
        }
    }
}