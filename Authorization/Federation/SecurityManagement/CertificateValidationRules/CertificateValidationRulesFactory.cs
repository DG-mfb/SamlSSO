
using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Reflection;
using Kernel.Security.Configuration;
using Kernel.Security.Validation;

namespace SecurityManagement.CertificateValidationRules
{
    internal class CertificateValidationRulesFactory
    {
        static CertificateValidationRulesFactory()
        {
            CertificateValidationRulesFactory.InstanceCreator = t => (ICertificateValidationRule)Activator.CreateInstance(t);
        }
        public static IEnumerable<ICertificateValidationRule> GetRules(CertificateValidationConfiguration configuration)
        {
            var rules = ReflectionHelper.GetAllTypes(new[] { typeof(CertificateValidationRule).Assembly }, t =>
            !t.IsAbstract && !t.IsInterface && typeof(ICertificateValidationRule).IsAssignableFrom(t))
            .Union(configuration.ValidationRules.Select(x => x.Type))
            .Select(t => CertificateValidationRulesFactory.InstanceCreator(t));
            return rules;
        }

        public static Func<Type, ICertificateValidationRule> InstanceCreator { get; set; }
    }
}