using System.Collections.Generic;
using Kernel.Data;

namespace Kernel.Security.Configuration
{
    public class BackchannelConfiguration
    {
        public TypeDescriptor BackchannelValidatorResolver { get; set; }
        public bool UsePinningValidation { get; set; }
        public IDictionary<string, IEnumerable<string>> Pins { get; set; }
    }
}