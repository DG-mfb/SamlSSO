using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel.Validation;

namespace Kernel.Federation.Protocols.Request
{
    public interface IRequestValidator<TContext> : IValidator
    {
        Task ValidateIRequest(TContext context, IDictionary<string, string> form);
    }
}