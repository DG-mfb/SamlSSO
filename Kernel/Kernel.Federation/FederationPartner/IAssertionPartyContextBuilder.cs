using System;

namespace Kernel.Federation.FederationPartner
{
    public interface IAssertionPartyContextBuilder : IDisposable
    {
        FederationPartyConfiguration BuildContext(string federationPartyId);
    }
}