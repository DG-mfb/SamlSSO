using System.Security.Claims;
using System.Threading.Tasks;

namespace Kernel.Authentication.Claims
{
    public interface IClaimsIdentityMapper<TResult>
    {
        Task<TResult> MapIClaimsIdentity(ClaimsIdentity identity);
    }
}