using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface IBindingHandler
    {
        Task HandleOutbound(SamlOutboundContext context);
        Task HandleInbound(SamlInboundContext context);
    }
}