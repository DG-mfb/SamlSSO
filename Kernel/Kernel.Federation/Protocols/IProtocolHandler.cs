using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface IProtocolHandler
    {
        Task HandleOutbound(SamlProtocolContext context);
        Task HandleInbound(SamlProtocolContext context);
    }
    public interface IProtocolHandler<TBinding> : IProtocolHandler where TBinding : IBindingHandler
    {
        
    }
}