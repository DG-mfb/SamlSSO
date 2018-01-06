using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Federation.Protocols.Response;
using Kernel.Logging;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class SamlTokenResponseParser : SamlResponseParser, IResponseParser<string, TokenResponse>
    {
        public SamlTokenResponseParser(ILogProvider logProvider):base(logProvider)
        {
           
        }
        public Task<TokenResponse> ParseResponse(string context)
        {
            var response = ParseInternal(context);
            return Task.FromResult(response);
        }

        private TokenResponse ParseInternal(string responseText)
        {
            var response = new TokenResponse();
            base.ReadResponseStatus(XmlReader.Create(new StringReader(responseText)), response);
            this.ReadToken(XmlReader.Create(new StringReader(responseText)), response);
            return response;
        }

        private void ReadToken(XmlReader reader, TokenResponse response)
        {
            var hasToken = TokenHelper.TryToMoveToToken(reader);
            if(hasToken)
            {
                var assertions = new List<XmlElement>();
                var doc = new XmlDocument();
                var el = doc.ReadNode(reader);
                response.Assertions = new[] { (XmlElement)el };
            }
        }
    }
}