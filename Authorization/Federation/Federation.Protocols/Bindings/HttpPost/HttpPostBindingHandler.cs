using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Federation.Protocols.Response;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;

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
            var responseDispatcher = this._dependencyResolver.Resolve<ResponseDispatcher>();
            await responseDispatcher.SendAsync();
        }
        
        public async Task HandleInbound(SamlInboundContext context)
        {
            var responseHandler = this._dependencyResolver.Resolve <IReponseHandler<ClaimsIdentity>>();
            var httpPostContext = context as HttpPostResponseContext;
            var result = await responseHandler.Handle(httpPostContext);
            httpPostContext.Result = result;
        }
    }
}