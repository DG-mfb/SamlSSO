﻿using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class SamlLogoutResponseParser : SamlResponseParser, IMessageParser<string, LogoutResponse>
    {
        public SamlLogoutResponseParser(ILogProvider logProvider):base(logProvider)
        {
        }

        public Task<LogoutResponse> Parse(string context)
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