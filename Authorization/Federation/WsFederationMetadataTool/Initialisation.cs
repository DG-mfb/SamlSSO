using System;
using System.Threading.Tasks;
using Kernel.Configuration;
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
                var dataSource = AppSettingsConfigurationManager.GetSetting("dataInitialiser", String.Empty);
                if (!String.IsNullOrWhiteSpace(dataSource))
                    initialiser.InitialiserTypes.Add(dataSource);
                var task = initialiser.Initialise(container)
                      .ContinueWith(t =>
                      {
                          throw t.Exception;
                      }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }
}