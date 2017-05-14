using System.Collections.Generic;

namespace Stickler.Engine
{
    public interface ICompiledRuleStore
    {
        IEnumerable<Rule<TTarget, TComparison>> GetRules<TTarget, TComparison>();
        Rule<TTarget, TComparison> AddRule<TTarget, TComparison>(RuleDto ruleDto);
        Rule<TTarget, TComparison> UpdateRule<TTarget, TComparison>(RuleDto ruleDto);
        void DeleteRule(RuleDto ruleDto);
    }
}
