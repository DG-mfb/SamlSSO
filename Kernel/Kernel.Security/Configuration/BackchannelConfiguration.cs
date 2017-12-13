using System.Collections.Generic;
using Kernel.Data;

namespace Kernel.Security.Configuration
{
    public class BackchannelConfiguration
    {
        public BackchannelConfiguration()
        {
            this.Pins = new Dictionary<PinType, IEnumerable<string>>();
        }
        public TypeDescriptor BackchannelValidatorResolver { get; set; }
        public bool UsePinningValidation { get; set; }
        public IDictionary<PinType, IEnumerable<string>> Pins { get; }
    }
}