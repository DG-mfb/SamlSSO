using System;
using System.Threading.Tasks;

namespace Kernel.Validation
{
    public interface IValidationRule
    {
        Task Validate(ValidationContext context, Func<ValidationContext, Task> next);
    }
}