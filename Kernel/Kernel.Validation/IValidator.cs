using System.Threading.Tasks;

namespace Kernel.Validation
{
    public interface IValidator
    {
        Task Validate(ValidationContext context);
    }
}