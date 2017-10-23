
using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Reflection;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace SecurityManagement.BackchannelCertificateValidationRules
{
    internal class BackchannelCertificateValidationRulesFactory
    {
        static BackchannelCertificateValidationRulesFactory()
        {
            BackchannelCertificateValidationRulesFactory.InstanceCreator = t => (IBackchannelCertificateValidationRule)Activator.CreateInstance(t);
            BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory = t => (ICertificateValidatorResolver)Activator.CreateInstance(t);
        }
        public static IEnumerable<IBackchannelCertificateValidationRule> GetRules(BackchannelConfiguration configuration)
        {
            var rules = ReflectionHelper.GetAllTypes(new[] { typeof(BackchannelValidationRule).Assembly }, t =>
            !t.IsAbstract && !t.IsInterface && typeof(IBackchannelCertificateValidationRule).IsAssignableFrom(t))
            .Select(t => BackchannelCertificateValidationRulesFactory.InstanceCreator(t));
            return rules;
        }

        public static Func<Type, IBackchannelCertificateValidationRule> InstanceCreator { get; set; }

        public static Func<Type, ICertificateValidatorResolver> CertificateValidatorResolverFactory { get; set; }
    }
}