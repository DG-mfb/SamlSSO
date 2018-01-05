using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;

namespace Federation.Protocols.Request
{
    internal class RequestTypeResolver : IMessageTypeResolver
    {
        public Type ResolveMessageType(string message)
        {
            throw new NotImplementedException();
        }
    }
}
