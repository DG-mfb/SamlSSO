using Kernel.Configuration;

namespace Federation.Metadata.HttpRetriever
{
    internal class CustomConfigurator : ICustomConfigurator<HttpDocumentRetriever>
    {
        public void Configure(HttpDocumentRetriever configurable)
        {
            //Customise document http rertiever here
        }
    }
}