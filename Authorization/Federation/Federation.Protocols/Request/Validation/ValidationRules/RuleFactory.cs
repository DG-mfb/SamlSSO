using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.DependancyResolver;
using Kernel.Reflection;
using Kernel.Validation;

namespace Federation.Protocols.Request.Validation.ValidationRules
{
    internal class RuleFactory : IValidationRuleFactory<RequestValidationRule>
    {
        private readonly IDependencyResolver _resolver;
        public RuleFactory(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        public IEnumerable<IValidationRule> GetValidationRules(Func<RequestValidationRule, IEnumerable<object>> resolver)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules(Func<RequestValidationRule, bool> filter)
        {
            var types = ReflectionHelper.GetAllTypes(t => !t.IsAbstract && !t.IsInterface && typeof(RequestValidationRule).IsAssignableFrom(t));
            var rules = types.Select(t => this._resolver.Resolve(t))
                .Cast<RequestValidationRule>();
            return rules;
        }

        public IEnumerable<IValidationRule> GetValidationRules(Type type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules()
        {
            return this.GetValidationRules(x => true);
        }
    }
}