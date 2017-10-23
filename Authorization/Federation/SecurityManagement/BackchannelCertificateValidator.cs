using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;
using SecurityManagement.BackchannelCertificateValidationRules;

namespace SecurityManagement
{
    internal class BackchannelCertificateValidator : IBackchannelCertificateValidator
    {
        private BackchannelConfiguration _configuration;
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
            var httpMessage = sender as HttpWebRequest;
            
            //
#if (DEBUG)
            if (httpMessage != null && httpMessage.Address.AbsoluteUri.Contains("dg-mfb"))
                return true;
#endif
            this._logProvider.LogMessage(String.Format("Validating backhannel certificate. sslPolicyErrors was: {0}", sslPolicyErrors));
            var federationPartyId = FederationPartyIdentifierHelper.GetFederationPartyIdFromRequestOrDefault(httpMessage);
            var configiration = this.GetConfiguration(federationPartyId);
            var context = new BackchannelCertificateValidationContext(certificate, chain, sslPolicyErrors);
            //if pinning validation is enabled it take precedence
            if (configiration.UsePinningValidation && configiration.BackchannelValidatorResolver != null)
            {
                _logProvider.LogMessage(String.Format("Pinning validation entered. Validator type: {0}", configiration.BackchannelValidatorResolver.Type));
                
                    var type = configiration.BackchannelValidatorResolver.Type;
                    var instance = BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory(type);
                if (instance != null)
                {
                    var validators = instance.Resolve<IPinningSertificateValidator>(federationPartyId)
                        .Where(x => x != null)
                        .ToList();

                    Func<object, BackchannelCertificateValidationContext, Task> seed1 = (o, c) => Task.CompletedTask;
                    var del = validators.Aggregate(seed1, (next, validator) => new Func<object, BackchannelCertificateValidationContext, Task>((o, c) => validator.Validate(o, c, next)));
                    var backChannelValidationTask = del(sender, context);
                    backChannelValidationTask.Wait();
                    return context.IsValid;
                }
            }

            //if pinning validation is desabled run validation rules if any
            //default rule. SslPolicyErrors no error vaidation. To ve reviewed
            Func<BackchannelCertificateValidationContext, Task> seed = x =>
            {
                if(!x.IsValid && x.SslPolicyErrors == SslPolicyErrors.None)
                    x.Validated();
                return Task.CompletedTask;
            };

            var rules = BackchannelCertificateValidationRulesFactory.GetRules(configiration);
            var validationDelegate = rules.Aggregate(seed, (f, next) => new Func<BackchannelCertificateValidationContext, Task>(c => next.Validate(c, f)));
            var task = validationDelegate(context);
            task.Wait();
            return context.IsValid;
        }
        
        private BackchannelConfiguration GetConfiguration(string federationPartyId)
        {
            if (this._configuration == null)
            {
                this._configuration = this._configurationProvider.GeBackchannelConfiguration(federationPartyId);
            }
            if (this._configuration == null)
                throw new InvalidOperationException("CertificateValidationConfiguration is null!");

            return this._configuration;
        }
    }
}