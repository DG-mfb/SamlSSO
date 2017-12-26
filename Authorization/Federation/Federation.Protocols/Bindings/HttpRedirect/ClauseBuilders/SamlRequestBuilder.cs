﻿using System;
using System.Threading.Tasks;
using Federation.Protocols.Request;
using Kernel.Federation.Protocols;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Bindings.HttpRedirect.ClauseBuilders
{
    internal class SamlRequestBuilder : IRedirectClauseBuilder
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

            var httpRedirectContext = context as HttpRedirectContext;
            if (httpRedirectContext == null)
                throw new InvalidOperationException(String.Format("Binding context must be of type:{0}. It was: {1}", typeof(HttpRedirectContext).Name, context.GetType().Name));
            var authnRequest = AuthnRequestHelper.BuildAuthnRequest(httpRedirectContext.AuthnRequestContext);

            var serialised = this._authnRequestSerialiser.Serialize(authnRequest);
            context.RequestParts.Add(HttpRedirectBindingConstants.SamlRequest, serialised);
            return Task.CompletedTask;
        }
    }
}