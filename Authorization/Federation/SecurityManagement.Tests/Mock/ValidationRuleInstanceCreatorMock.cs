using System;
using System.Linq.Expressions;
using Kernel.Cryptography.Validation;
using Kernel.Logging;

namespace SecurityManagement.Tests.Mock
{
    internal class ValidationRuleInstanceCreatorMock
    {
        public static ICertificateValidationRule CreateInstance(Type t)
        {
            var logger = new LogProviderMock();
            var par = Expression.Parameter(typeof(ILogProvider));
            var ctor = t.GetConstructor(new[] { typeof(ILogProvider) });
            if (ctor == null)
                return (ICertificateValidationRule)Activator.CreateInstance(t);
            var newEx = Expression.New(ctor, par);
            var lambda = Expression.Lambda<Func<ILogProvider, ICertificateValidationRule>>(newEx, par)
                .Compile();
            return lambda(logger);
        }
    }
}