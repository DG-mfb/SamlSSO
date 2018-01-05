using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Logging;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Request
{
    internal class PostRequestDispatcher : ISamlMessageDespatcher<HttpPostRequestContext>
    {
        private readonly Func<IEnumerable<ISamlClauseBuilder>> _buildesFactory;
        private readonly ILogProvider _logProvider;
        
        public PostRequestDispatcher(Func<IEnumerable<IPostClauseBuilder>> buildesFactory, ILogProvider logProvider)
        {
            this._buildesFactory = buildesFactory;
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
            var builders = this._buildesFactory().OrderBy(x => x.Order);
            foreach(var b in builders)
            {
                await b.Build(context.BindingContext);
            }
            var request = context.BindingContext.RequestParts[HttpRedirectBindingConstants.SamlRequest];
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(request));

            var relyingStateSerialised = context.BindingContext.RequestParts[HttpRedirectBindingConstants.RelayState];
            this._logProvider.LogMessage(String.Format("Building SAML form. Destination url: {0}", context.BindingContext.DestinationUri.AbsoluteUri));
            
            context.Form.ActionURL = context.BindingContext.DestinationUri.AbsoluteUri;
            context.Form.SetRequest(base64Encoded);
            context.Form.SetRelatState(relyingStateSerialised);
            var samlForm = context.Form;
            this._logProvider.LogMessage(String.Format("Despatching saml form./r/n. {0}", samlForm));
            await context.DespatchDelegate(samlForm);
        }
    }
}