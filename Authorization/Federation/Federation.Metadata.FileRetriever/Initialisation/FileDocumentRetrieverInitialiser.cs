using System.Threading.Tasks;
using Kernel.DependancyResolver;
using Shared.Initialisation;

namespace Federation.Metadata.FileRetriever.Initialisation
{
    public class FileDocumentRetrieverInitialiser : Initialiser
    {
        public override byte Order
        {
            get { return 1; }
        }

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<FileDocumentRetriever>(Lifetime.Transient);
            return Task.CompletedTask;
        }
    }
}