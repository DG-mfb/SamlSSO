using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings
{
    public interface IBindingDecoder<TRequest> : IBindingDecoder
    {
        Task<SamlInboundMessage> Decode(TRequest request);
    }
}