using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.Federation.Protocols
{
    public interface IRelayStateAppender
    {
        Task BuildRelayState(AuthnRequestContext authnRequestContext);
    }
}