using System.IdentityModel.Tokens;
using Kernel.Federation.Tokens;

namespace Federation.Protocols.Tokens.Validation
{
    internal class ConditionsValidator : ITokenClauseValidator<Saml2Conditions>
    {
        public void Validate(Saml2Conditions clause)
        {
            //do nothing for now
        }
    }
}