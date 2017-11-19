using System;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public abstract class SamlOutboundContext<T> : SamlOutboundContext
    {
        public abstract Func<T, Task> DespatchDelegate { get; set; }
    }
}