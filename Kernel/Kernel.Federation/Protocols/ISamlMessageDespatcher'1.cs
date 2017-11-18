using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface ISamlMessageDespatcher<in TContext> : ISamlMessageDespatcher where TContext : SamlOutboundContext
    {
        Task SendAsync(TContext context);
    }
}