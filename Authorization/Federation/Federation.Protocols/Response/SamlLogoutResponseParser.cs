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
    internal class SamlLogoutResponseParser : SamlResponseParser, IResponseParser<string, LogoutResponse>
    {
        public SamlLogoutResponseParser(ILogProvider logProvider):base(logProvider)
        {
        }

        public Task<LogoutResponse> ParseResponse(string context)
        {
            var response = ParseResponseInternal(context);
            return Task.FromResult(response);
        }

        protected override StatusResponse ParseInernal(string response)
        {
            return this.ParseResponseInternal(response);
        }

        private LogoutResponse ParseResponseInternal(string responseText)
        {
            var response = new LogoutResponse();
            base.ReadResponseStatus(XmlReader.Create(new StringReader(responseText)), response);
            return response;
        }
    }
}