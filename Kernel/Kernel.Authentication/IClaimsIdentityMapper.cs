using System.Security.Claims;
using System.Threading.Tasks;

namespace Kernel.Authentication
{
    public interface IClaimsIdentityMapper<TResult>
    {
        Task<TResult> MapClaimsIdentity(ClaimsIdentity identity);
    }
}