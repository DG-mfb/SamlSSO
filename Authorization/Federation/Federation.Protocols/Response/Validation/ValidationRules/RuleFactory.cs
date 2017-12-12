using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.DependancyResolver;
using Kernel.Reflection;
using Kernel.Validation;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class RuleFactory : IValidationRuleFactory<ResponseValidationRule>
    {
        private readonly IDependencyResolver _resolver;
        public RuleFactory(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        public IEnumerable<IValidationRule> GetValidationRules(Func<ResponseValidationRule, IEnumerable<object>> resolver)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules(Func<ResponseValidationRule, bool> filter)
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(ResponseValidationRule).IsAssignableFrom(t));
            return types.Select(t => this._resolver.Resolve(t))
                .Cast<ResponseValidationRule>()
                .Where(filter); 
        }

        public IEnumerable<IValidationRule> GetValidationRules(Type type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules()
        {
            throw new NotImplementedException();
        }
    }
}