using System.Threading.Tasks;
using Kernel.Validation;

namespace Kernel.Federation.Protocols.Request
{
    public interface IRequestValidator<TRequest> : IValidator
    {
        Task ValidateIRequest(TRequest request);
    }
}