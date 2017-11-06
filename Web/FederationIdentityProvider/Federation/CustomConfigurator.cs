using Kernel.Configuration;
using Kernel.Web;

namespace FederationIdentityProvider.Federation
{
    internal class CustomConfigurator : ICustomConfigurator<IDocumentRetriever>
    {
        public void Configure(IDocumentRetriever configurable)
        {
            configurable.RequireHttps = false;
        }
    }
}