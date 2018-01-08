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
        private readonly Func<IEnumerable<ResponseValidationRule>> _resolver;
        public RuleFactory(Func<IEnumerable<ResponseValidationRule>> resolver)
        {
            this._resolver = resolver;
        }
        public IEnumerable<IValidationRule> GetValidationRules(Func<ResponseValidationRule, IEnumerable<object>> resolver)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules(Func<ResponseValidationRule, bool> filter)
        {
            var rules = this._resolver();
            
            return rules.Where(r => r.Scope == RuleScope.Always)
            .ToList()
            .Union(rules.Where(filter));
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