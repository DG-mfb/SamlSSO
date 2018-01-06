using System;
using System.Collections.Generic;

namespace Kernel.Federation.Protocols
{
    public interface IMessageTypeResolver
    {
        Type ResolveMessageType(string message, IEnumerable<Type> types);
    }
}