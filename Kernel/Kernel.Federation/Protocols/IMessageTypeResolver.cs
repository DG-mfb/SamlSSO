using System;

namespace Kernel.Federation.Protocols
{
    public interface IMessageTypeResolver
    {
        Type ResolveMessageType(string message);
    }
}