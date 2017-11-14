using Kernel.Configuration;
using Kernel.Web;

namespace FederationIdentityProvider.Federation
{
    internal class CustomConfigurator : ICustomConfigurator<IHttpDocumentRetriever>
    {
        public void Configure(IHttpDocumentRetriever configurable)
        {
            configurable.RequireHttps = false;
        }
    }
}