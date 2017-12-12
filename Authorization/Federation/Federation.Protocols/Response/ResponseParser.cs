﻿using System;
using System.Text;
using System.Threading.Tasks;
using Federation.Protocols.Response.Validation;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;
using Kernel.Federation.Protocols.Response;
using Kernel.Initialisation;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class ResponseParser : IResponseParser<HttpPostResponseContext, ResponseStatus>
    {
        private readonly ILogProvider _logProvider;

        public ResponseParser(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }
        public async Task<ResponseStatus> ParseResponse(HttpPostResponseContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            this._logProvider.LogMessage(String.Format("Response received:\r\n {0}", responseText));
            var responseStatus = ResponseHelper.ParseResponseStatus(responseText, this._logProvider);
            var service = ApplicationConfiguration.Instance.DependencyResolver.Resolve<ResponseValidator>();
            await service.ValidateResponse(responseStatus, elements);
            return responseStatus;
        }
    }
}