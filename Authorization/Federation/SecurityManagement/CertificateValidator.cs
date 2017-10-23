using System;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;
using SecurityManagement.CertificateValidationRules;

namespace SecurityManagement
{
    internal class CertificateValidator : X509CertificateValidator, ICertificateValidator
    {
        private CertificateValidationConfiguration _configuration;
        private readonly ILogProvider _logProvider;
        private readonly ICertificateValidationConfigurationProvider _configurationProvider;

        public CertificateValidator(ICertificateValidationConfigurationProvider configurationProvider, ILogProvider logProvider)
        {
            if (configurationProvider == null)
                throw new ArgumentNullException("configurationProvider");

            this._configurationProvider = configurationProvider;
            this._logProvider = logProvider;
        }

        public string FederationPartyId { get; private set; }

        public X509CertificateValidationMode X509CertificateValidationMode
        {
            get
            {
                if (this._configuration == null)
                {
                    this._configuration = this.GetConfiguration();
                    if (this._configuration == null)
                        throw new ArgumentNullException("certificateValidationConfiguration");
                }
                return this._configuration.X509CertificateValidationMode;
            }
        }
        
        public void SetFederationPartyId(string federationPartyId)
        {
            this.FederationPartyId = federationPartyId;
        }

        public override void Validate(X509Certificate2 certificate)
        {
            this._logProvider.LogMessage(String.Format("Validating certificate: {0}", certificate.Subject));
            var configiration = this.GetConfiguration();
            var context = new CertificateValidationContext(certificate);
            Func<CertificateValidationContext, Task> seed = x => Task.CompletedTask;

            var rules = CertificateValidationRulesFactory.GetRules(configiration);
            var validationDelegate = rules.Aggregate(seed, (f, next) => new Func<CertificateValidationContext, Task>(c => next.Validate(c, f)));
            var task = validationDelegate(context);
            task.Wait();
        }
        
        private CertificateValidationConfiguration GetConfiguration()
        {
            if (this._configuration == null)
            {
                this._configuration = this._configurationProvider.GetConfiguration(this.FederationPartyId);
            }
            if (this._configuration == null)
                throw new InvalidOperationException("CertificateValidationConfiguration is null!");

            return this._configuration;
        }
    }
}