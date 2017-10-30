using System.Threading.Tasks;
using Kernel.Initialisation;
using Kernel.Logging;

namespace WsFederationMetadataTool
{
    internal static class Initialisation
    {
        public static async Task Init()
        {
            await Initialisation.InitializeServer();
        }
        private static async Task InitializeServer()
        {
            using (new InformationLogEventWriter())
            {
                var container = ApplicationConfiguration.Instance.DependencyResolver;
                var initialiser = ApplicationConfiguration.Instance.ServerInitialiserFactory();
                var task = initialiser.Initialise(container);
                await task;
            }
        }
    }
}