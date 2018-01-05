using System.Threading.Tasks;
using Kernel.Federation.Protocols.Request;

namespace Kernel.Federation.Protocols
{
    public interface IRelayStateAppender
    {
        Task BuildRelayState(RequestContext authnRequestContext);
    }
}