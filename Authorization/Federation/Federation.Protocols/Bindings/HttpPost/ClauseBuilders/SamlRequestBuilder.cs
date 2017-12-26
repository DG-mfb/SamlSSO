using System;
using System.Threading.Tasks;
using Federation.Protocols.Request;
using Kernel.Federation.Protocols;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpPost.ClauseBuilders
{
    internal class SamlRequestBuilder : IPostClauseBuilder
    {
        private readonly IAuthnRequestSerialiser _authnRequestSerialiser;
        public SamlRequestBuilder(IAuthnRequestSerialiser authnRequestSerialiser)
        {
            this._authnRequestSerialiser = authnRequestSerialiser;
        }
        public uint Order { get { return 0; } }

        public Task Build(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var requestContext = ((RequestBindingContext)context).AuthnRequestContext;
            var request = AuthnRequestHelper.BuildAuthnRequest(requestContext);

            var serialised = this._authnRequestSerialiser.Serialize(request);
            context.RequestParts.Add(HttpRedirectBindingConstants.SamlRequest, serialised);
            return Task.CompletedTask;
        }
    }
}