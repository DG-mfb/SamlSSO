using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Models;

namespace Federation.Protocols.Request.Parsers
{
    internal class LogoutRequestParser : SamlRequestParser, IMessageParser<string, LogoutRequest>
    {
        private readonly IRequestSerialiser _requestSerialiser;
        public LogoutRequestParser(IRequestSerialiser requestSerialiser, ILogProvider logProvider)
            :base(logProvider)
        {
            this._requestSerialiser = requestSerialiser;
        }

        Task<LogoutRequest> IMessageParser<string, LogoutRequest>.Parse(string context)
        {
            var request = this.ParseInernal(context);
            return Task.FromResult((LogoutRequest)request);
        }

        protected override RequestAbstract ParseInernal(string data)
        {
            var request = this._requestSerialiser.Deserialize<LogoutRequest>(data);
            return request;
        }
    }
}