using System;
using System.Collections.Generic;
using Kernel.Validation;

namespace Federation.Protocols.Request.Validation.ValidationRules
{
    internal class RuleFactory : IValidationRuleFactory<RequestValidationRule>
    {
        private readonly Func<IEnumerable<RequestValidationRule>> _resolver;
        public RuleFactory(Func<IEnumerable<RequestValidationRule>> resolver)
        {
            this._resolver = resolver;
        }
        public IEnumerable<IValidationRule> GetValidationRules(Func<RequestValidationRule, IEnumerable<object>> resolver)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationRule> GetValidationRules(Func<RequestValidationRule, bool> filter)
        {
            var rules = this._resolver();
            
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