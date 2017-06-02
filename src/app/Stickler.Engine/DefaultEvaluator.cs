using System;

namespace Stickler.Engine
{
    public class DefaultEvaluator : IEvaluator
    {
        private readonly ICompiledRuleStore _store;

        public DefaultEvaluator()
            : this(new DefaultCompiledRuleStore())
        {
        }

        public DefaultEvaluator(ICompiledRuleStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            
            _store = store;
        }

        public ResultSet Evaluate<TTarget, TComparison>(TTarget target, TComparison comparison)
        {
            var rules = _store.GetRules<TTarget, TComparison>();
            
            var resultSet = new ResultSet
            {
                Status = ResultStatus.Pass
            };

            foreach (var rule in rules)
            {
                var result = new Result
                {
                    RuleName = rule.Name,
                    RuleExpression = rule.Expression,
                    Status = ResultStatus.Pass
                };

                if (!rule.Validate(target, comparison))
                {
                    result.Status = ResultStatus.Fail;
                    resultSet.Status = ResultStatus.Fail;
                }

                resultSet.Results.Add(result);
            }

            return resultSet;
        }
    }
}

