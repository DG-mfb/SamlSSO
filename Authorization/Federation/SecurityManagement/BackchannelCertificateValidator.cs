using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Cryptography.Validation;
using Kernel.Logging;
using SecurityManagement.BackchannelCertificateValidationRules;

namespace SecurityManagement
{
    internal class BackchannelCertificateValidator : IBackchannelCertificateValidator
    {
        private CertificateValidationConfiguration _configuration;
        private readonly ILogProvider _logProvider;
        private readonly ICertificateValidationConfigurationProvider _configurationProvider;

        public BackchannelCertificateValidator(ICertificateValidationConfigurationProvider configurationProvider, ILogProvider logProvider)
        {
            if (configurationProvider == null)
                throw new ArgumentNullException("configurationProvider");
            this._logProvider = logProvider;
            this._configurationProvider = configurationProvider;
        }

        public bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            this._logProvider.LogMessage(String.Format("Validating backhannel certificate. sslPolicyErrors was: {0}", sslPolicyErrors));
            var federationPartyId = FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(sender as HttpWebRequest);
            var configiration = this.GetConfiguration(federationPartyId);
            
            //ToDo: complete pinning validation. Moved to back log on 27/09/2017
            if(configiration.UsePinningValidation && configiration.BackchannelValidatorResolver != null)
            {
                _logProvider.LogMessage(String.Format("Pinning validation entered. Validator type: {0}", configiration.BackchannelValidatorResolver.Type));
                try
                {
                    var type = configiration.BackchannelValidatorResolver.Type;
                    var instance = Activator.CreateInstance(type) as ICertificateValidatorResolver;
                    if(instance != null)
                    {
                        var validators = instance.Resolve<IBackchannelCertificateValidator>();
                        
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    Exception innerEx;
                    this._logProvider.TryLogException(ex, out innerEx);
                    this._logProvider.LogMessage(String.Format("Despite an error occurred validation passed. Error: {0}", ex.Message));
                    return true;
                }
            }

            var context = new BackchannelCertificateValidationContext(certificate, chain, sslPolicyErrors);

            //default rule. No validation
            Func<BackchannelCertificateValidationContext, Task> seed = x =>
            {
                x.Validated();
                return Task.CompletedTask;
            };

            var rules = BackchannelCertificateValidationRulesFactory.GetRules(configiration);
            var validationDelegate = rules.Aggregate(seed, (f, next) => new Func<BackchannelCertificateValidationContext, Task>(c => next.Validate(c, f)));
            var task = validationDelegate(context);
            task.Wait();
            return context.IsValid;
        }
        
        private CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            if (this._configuration == null)
            {
                this._configuration = this._configurationProvider.GetConfiguration(federationPartyId);
            }
            if (this._configuration == null)
                throw new InvalidOperationException("CertificateValidationConfiguration is null!");

            return this._configuration;
        }
    }
}