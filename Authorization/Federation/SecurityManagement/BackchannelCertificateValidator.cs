using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Kernel.Logging;
using Kernel.Security.Validation;
using SecurityManagement.BackchannelCertificateValidationRules;

namespace SecurityManagement
{
    /// <summary>
    /// Backchannel certificate validator. Validates remote certificate for https.
    /// Perform validation as follows: Locates pinning validators if enabled and invoke all in chain.
    /// if validated no more validation is perfrmed, run custom validation rules otherwise
    /// </summary>
    internal class BackchannelCertificateValidator : IBackchannelCertificateValidator
    {
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
            
            this._logProvider.LogMessage(String.Format("Validating backhannel certificate. sslPolicyErrors was: {0}", sslPolicyErrors));
            
            var configiration = this._configurationProvider.GeBackchannelConfiguration(httpMessage.RequestUri);
            var context = new BackchannelCertificateValidationContext(certificate, chain, sslPolicyErrors);
            
            //if pinning validation is enabled it take precedence
            if (configiration.UsePinningValidation && configiration.BackchannelValidatorResolver != null)
            {
                _logProvider.LogMessage(String.Format("Pinning validation entered. Validator type: {0}", configiration.BackchannelValidatorResolver.Type));

                var type = configiration.BackchannelValidatorResolver.Type;
                var instance = BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory(type);
                if (instance != null)
                {
                    var validators = instance.Resolve(configiration)
                        .Where(x => x != null)
                        .ToList();

                    Func<object, BackchannelCertificateValidationContext, Task> seed1 = (o, c) => Task.CompletedTask;
                    var del = validators.Aggregate(seed1, (next, validator) => new Func<object, BackchannelCertificateValidationContext, Task>((o, c) => validator.Validate(o, c, next)));
                    var backChannelValidationTask = del(sender, context);
                    backChannelValidationTask.Wait();
                    return context.IsValid;
                }
            }

            //if pinning validation is disabled run validation rules if any
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
    }
}