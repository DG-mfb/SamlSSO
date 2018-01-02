using System;
using System.Threading.Tasks;
using System.Web.Http;
using Kernel.Configuration;
using Kernel.Initialisation;
using Kernel.Logging;
using ServerInitialisation;
using UnityResolver;
using WebApi.App_Start;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            using (new InformationLogEventWriter())
            {
                ApplicationConfiguration.RegisterDependencyResolver(() => new UnityDependencyResolver());
                ApplicationConfiguration.RegisterServerInitialiserFactory(() => new ServerInitialiser());

                //this.RegisterWebConfiguration();
                this.InitializeServer();
            }
        }

        private void InitializeServer()
        {
            using (new InformationLogEventWriter())
            {
                var container = ApplicationConfiguration.Instance.DependencyResolver;
                DIRegistration.Register(container);
                var initialiser = ApplicationConfiguration.Instance.ServerInitialiserFactory();
                var dataSource = AppSettingsConfigurationManager.GetSetting("dataInitialiser", String.Empty);
                if(!String.IsNullOrWhiteSpace(dataSource))
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