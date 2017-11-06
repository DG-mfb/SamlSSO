using Kernel.Configuration;
using Kernel.Web;

namespace Federation.Metadata.HttpRetriever
{
    internal class CustomConfigurator : ICustomConfigurator<IDocumentRetriever>
    {
        public void Configure(IDocumentRetriever configurable)
        {
            //Customise document http rertiever here
        }
    }
}