using System.Threading.Tasks;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;

namespace Kernel.Federation.Protocols
{
    public interface IInboundHandler<TResult>
    {
        Task<TResult> Handle(SamlInboundContext context);
    }
}