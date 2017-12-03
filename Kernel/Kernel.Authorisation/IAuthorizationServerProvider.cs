using System.Threading.Tasks;
using Kernel.Authorisation.Contexts;

namespace Kernel.Authorisation
{
    public interface IAuthorizationServerProvider
    {
        Task TokenEndpointResponse<TContext>(TContext context) where TContext : class, ITokenEndpointResponseContext;
    }
}