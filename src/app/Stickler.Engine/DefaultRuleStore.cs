using System;
using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class DefaultRuleStore : IRuleStore
    {
        private readonly IDictionary<string, RuleDto> _ruleRecords;

        public DefaultRuleStore()
        {
            _ruleRecords = new Dictionary<string, RuleDto>();
        }

        public IEnumerable<RuleDto> GetRules<TTarget, TComparison>()
        {
            var targetTypeName = typeof(TTarget).Name;
            
            return _ruleRecords
                .Values
                .Where(r => string.Equals(r.TargetTypeName, targetTypeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public RuleDto AddRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var toAdd = ruleDto.Clone();

            if (string.IsNullOrWhiteSpace(ruleDto.Id))
                toAdd.Id = Guid.NewGuid().ToString();

            _ruleRecords.Add(toAdd.Id, toAdd);

            return toAdd;
        }

        public RuleDto UpdateRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var toUpdate = ruleDto.Clone();

            if (ruleDto.Id == null || !_ruleRecords.ContainsKey(toUpdate.Id))
                return AddRule<TTarget, TComparison>(toUpdate);

            DeleteRule(toUpdate);
            return AddRule<TTarget, TComparison>(toUpdate);
        }

        public void DeleteRule(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var toDelete = ruleDto.Clone();

            _ruleRecords.Remove(toDelete.Id);
        }
    }
}
