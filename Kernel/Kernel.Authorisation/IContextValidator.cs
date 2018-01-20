using System.Threading.Tasks;

namespace Kernel.Authorisation
{
    public interface IContextValidator<TContext>
    {
        Task ValidateContext(TContext context);
    }
}