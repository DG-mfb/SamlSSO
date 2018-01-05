using System.Threading.Tasks;
using Kernel.Federation.Protocols.Bindings.HttpPostBinding;

namespace Kernel.Federation.Protocols
{
    public interface IInboundHandler<TContext>
    {
        Task Handle(TContext context);
    }
}