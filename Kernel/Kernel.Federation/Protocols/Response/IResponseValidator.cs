using System.Threading.Tasks;
using Kernel.Validation;

namespace Kernel.Federation.Protocols.Response
{
    public interface IResponseValidator<TResponse> : IValidator
    {
        Task ValidateResponse(TResponse response);
    }
}