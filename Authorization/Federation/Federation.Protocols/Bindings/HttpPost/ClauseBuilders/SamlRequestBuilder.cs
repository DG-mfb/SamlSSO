using System;
using System.Threading.Tasks;
using Federation.Protocols.Request;
using Kernel.Federation.Constants;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Bindings.HttpPost.ClauseBuilders
{
    internal class SamlRequestBuilder : IPostClauseBuilder
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
            var requestContext = ((RequestPostBindingContext)context).RequestContext;
            var request = RequestHelper.BuildRequest(requestContext);

            var serialised = this._authnRequestSerialiser.Serialize(request);
            context.RequestParts.Add(HttpRedirectBindingConstants.SamlRequest, serialised);
            return Task.CompletedTask;
        }
    }
}