using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.Parsers
{
    internal class AuthnRequestParser : SamlRequestParser, IMessageParser<string, AuthnRequest>
    {
        private readonly IRequestSerialiser _requestSerialiser;
        public AuthnRequestParser(IRequestSerialiser requestSerialiser, ILogProvider logProvider)
            :base(logProvider)
        {
            this._requestSerialiser = requestSerialiser;
        }

        Task<AuthnRequest> IMessageParser<string, AuthnRequest>.Parse(string context)
        {
            var request = this.ParseInernal(context);
            return Task.FromResult((AuthnRequest)request);
        }

        protected override RequestAbstract ParseInernal(string data)
        {
            var request = this._requestSerialiser.Deserialize<AuthnRequest>(data);
            return request;
        }
    }
}