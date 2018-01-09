using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Federation.Protocols.Tokens;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response
{
    internal class SamlTokenResponseParser : SamlResponseParser, IMessageParser<string, TokenResponse>
    {
        public SamlTokenResponseParser(ILogProvider logProvider):base(logProvider)
        {
        }
        public new Task<TokenResponse> Parse(string context)
        {
            var response = ParseResponseInternal(context);
            return Task.FromResult(response);
        }

        protected override StatusResponse ParseInernal(string response)
        {
            return this.ParseResponseInternal(response);
        }

        private TokenResponse ParseResponseInternal(string responseText)
        {
            var response = new TokenResponse();
            base.ReadResponseStatus(XmlReader.Create(new StringReader(responseText)), response);
            this.ReadToken(responseText, response);
            return response;
        }

        private void ReadToken(string responseText, TokenResponse response)
        {
            var hasToken = this.HasToken(responseText);
            if (hasToken)
            {
                var assertions = new List<XmlElement>();
                var doc = new XmlDocument();
                doc.LoadXml(responseText);
                var el = doc.DocumentElement;
                response.Assertions = new[] { (XmlElement)el };
            }
        }

        private bool HasToken(string response)
        {
            using (var reader = XmlReader.Create(new StringReader(response)))
            {
                var hasToken = TokenHelper.TryToMoveToToken(reader);
                return hasToken;
            }
        }
    }
}