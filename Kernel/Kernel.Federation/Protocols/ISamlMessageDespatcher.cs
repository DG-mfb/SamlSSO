using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface ISamlMessageDespatcher
    {
        Task SendAsync(SamlOutboundContext context);
    }
}