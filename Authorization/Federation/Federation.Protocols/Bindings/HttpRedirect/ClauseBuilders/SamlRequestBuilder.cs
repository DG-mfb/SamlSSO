using System;
using System.Threading.Tasks;
using Federation.Protocols.Request;
using Kernel.Federation.Protocols;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class SamlRequestBuilder : IRedirectClauseBuilder
    {
        private readonly IRequestSerialiser _authnRequestSerialiser;
        public SamlRequestBuilder(IRequestSerialiser authnRequestSerialiser)
        {
            this._authnRequestSerialiser = authnRequestSerialiser;
        }
        public uint Order { get { return 0; } }

        public Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpRedirectContext = context as RequestBindingContext;
            if (httpRedirectContext == null)
                throw new InvalidOperationException(String.Format("Binding context must be of type:{0}. It was: {1}", typeof(RequestBindingContext).Name, context.GetType().Name));
            var authnRequest = RequestHelper.BuildRequest(httpRedirectContext.RequestContext);

            var serialised = this._authnRequestSerialiser.Serialize(authnRequest);
            context.RequestParts.Add(HttpRedirectBindingConstants.SamlRequest, serialised);
            return Task.CompletedTask;
        }
    }
}