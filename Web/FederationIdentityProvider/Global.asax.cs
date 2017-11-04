using System.Web.Http;
using FederationIdentityProvider.App_Start;
using Kernel.Initialisation;
using Kernel.Logging;
using ServerInitialisation;
using UnityResolver;

namespace FederationIdentityProvider
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
                var task = initialiser.Initialise(container);
            }
        }
    }
}