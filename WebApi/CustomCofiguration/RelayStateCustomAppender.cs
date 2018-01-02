﻿using System;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using SSOOwinMiddleware;

namespace WebApi.CustomCofiguration
{
    public class RelayStateCustomAppender : IRelayStateAppender
    {
        public Task BuildRelayState(RequestContext authnRequestContext)
        {
            var owinRequest = authnRequestContext as OwinAuthnRequestContext;
            if (owinRequest != null)
            {
                var query = owinRequest.Context.Request.Query;
                var returnUri = query.Get("returnUrl");
                if (!String.IsNullOrWhiteSpace(returnUri))
                    owinRequest.RelyingState.Add("returnUrl", returnUri);
            }
            return Task.CompletedTask;
        }
    }
}