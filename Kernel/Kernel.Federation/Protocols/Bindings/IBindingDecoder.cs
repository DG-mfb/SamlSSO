using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings
{
    public interface IBindingDecoder<TRequest>
    {
        Task<IDictionary<string, object>> Decode(TRequest request);
    }
}