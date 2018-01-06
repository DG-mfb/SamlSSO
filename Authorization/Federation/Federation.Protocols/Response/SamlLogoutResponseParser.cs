using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.Protocols;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Shared.Federtion.Constants;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class SamlLogoutResponseParser : SamlResponseParser, IResponseParser<SamlInboundContext, LogoutResponse>
    {
        public SamlLogoutResponseParser(ILogProvider logProvider):base(logProvider)
        {
        }

        public Task<LogoutResponse> ParseResponse(SamlInboundContext context)
        {
            var elements = context.Form;
            var responseBase64 = elements[HttpRedirectBindingConstants.SamlResponse];
            var responseBytes = Convert.FromBase64String(responseBase64);
            var responseText = Encoding.UTF8.GetString(responseBytes);
            base.LogProvider.LogMessage(String.Format("Response received:\r\n {0}", responseText));
            var response = ParseInternal(responseText);
            return Task.FromResult(response);
        }

        private LogoutResponse ParseInternal(string responseText)
        {
            var response = new LogoutResponse();
            base.ReadResponseStatus(XmlReader.Create(new StringReader(responseText)), response);
            return response;
        }
    }
}