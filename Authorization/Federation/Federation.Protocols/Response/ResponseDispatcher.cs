using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Federation.Protocols.Response
{
    internal class ResponseDispatcher
    {
        private readonly ResponseBuilder _responseBuilder;
        public ResponseDispatcher(ResponseBuilder responseBuilder)
        {
            this._responseBuilder = responseBuilder;
        }
        public Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}