using System.Threading.Tasks;

namespace Kernel.Authorisation
{
    public interface ITokenGrantService<TContext>
    {
        Task GrantToken(TContext context);
    }
}