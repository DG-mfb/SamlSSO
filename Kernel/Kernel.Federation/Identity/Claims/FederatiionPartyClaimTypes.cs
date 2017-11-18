using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Federation.Identity.Claims
{
    public class FederatiionPartyClaimTypes
    {
        public string FederationParnerId { get; }
        IEnumerable<Claim> ClaimTypes { get; }
    }
}