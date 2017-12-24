using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Logging;

namespace Federation.Protocols.Request
{
    internal class PostRequestDispatcher : ISamlMessageDespatcher<HttpPostRequestContext>
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly ILogProvider _logProvider;
        
        public PostRequestDispatcher(IDependencyResolver dependencyResolver, ILogProvider logProvider)
        {
            this._dependencyResolver = dependencyResolver;
            this._logProvider = logProvider;
        }
        public Task SendAsync(SamlOutboundContext context)
        {
            return this.SendAsync(context as HttpPostRequestContext);
        }

        public async Task SendAsync(HttpPostRequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var builders = this.GetBuilders().OrderBy(x => x.Order);
            foreach(var b in builders)
            {
                await b.Build(context.BindingContext);
            }
            
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(context.BindingContext.Request.OuterXml));

            var relyingStateSerialised = context.BindingContext.State;
            this._logProvider.LogMessage(String.Format("Building SAML form. Destination url: {0}", context.BindingContext.DestinationUri.AbsoluteUri));
            
            context.Form.ActionURL = context.BindingContext.DestinationUri.AbsoluteUri;
            context.Form.SetRequest(base64Encoded);
            context.Form.SetRelatState(relyingStateSerialised);
            var samlForm = context.Form;
            this._logProvider.LogMessage(String.Format("Despatching saml form./r/n. {0}", samlForm));
            await context.DespatchDelegate(samlForm);
        }

        private IEnumerable<ISamlClauseBuilder> GetBuilders()
        {
            return this._dependencyResolver.ResolveAll<IPostClauseBuilder>();
        }
    }
}