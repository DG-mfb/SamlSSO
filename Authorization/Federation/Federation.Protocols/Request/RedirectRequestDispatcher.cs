using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Bindings.HttpRedirectBinding;

namespace Federation.Protocols.Request
{
    internal class RedirectRequestDispatcher : ISamlMessageDespatcher<HttpRedirectRequestContext>
    {
        private readonly Func<IEnumerable<ISamlClauseBuilder>> _buildesFactory;

        public RedirectRequestDispatcher(Func<IEnumerable<IRedirectClauseBuilder>> buildesFactory)
        {
            this._buildesFactory = buildesFactory;
        }

        public Task SendAsync(SamlOutboundContext context)
        {
            return this.SendAsync(context as HttpRedirectRequestContext);
        }

        public async Task SendAsync(HttpRedirectRequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var builders = this._buildesFactory();
            foreach (var b in builders.OrderBy(x => x.Order))
            {
                await b.Build(context.BindingContext);
            }
           
            await context.DespatchDelegate(context.BindingContext.GetDestinationUrl());
        }
    }
}