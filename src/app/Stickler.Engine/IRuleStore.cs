using System.Collections.Generic;

namespace Stickler.Engine
{
    public interface IRuleStore
    {
        IEnumerable<RuleDto> GetRules<TTarget, TComparison>();
        RuleDto AddRule<TTarget, TComparison>(RuleDto ruleDto);
        RuleDto UpdateRule<TTarget, TComparison>(RuleDto ruleDto);
        void DeleteRule(RuleDto ruleDto);
    }
}
