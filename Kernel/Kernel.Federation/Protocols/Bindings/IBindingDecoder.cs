using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols.Bindings
{
    public interface IBindingDecoder
    {
        Task<KeyValuePair<string, object>> DecodeElement(KeyValuePair<string, string> element);
    }
}