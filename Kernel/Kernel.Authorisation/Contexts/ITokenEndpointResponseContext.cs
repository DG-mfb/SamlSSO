using System.Collections.Generic;

namespace Kernel.Authorisation.Contexts
{
    public interface ITokenEndpointResponseContext
    {
        string Token { get; }
        IDictionary<string, object> RelayState { get; }
        bool IsRequestCompleted { get; }
        void RequestCompleted();
    }
}