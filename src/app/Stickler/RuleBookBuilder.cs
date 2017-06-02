using System;
using Stickler.Engine;

namespace Stickler
{
    public class RuleBookBuilder<TTarget, TComparison>
    {
        private readonly IRuleStore _ruleStore;

        private dynamic _onSuccess;
        private dynamic _onFailure;
        private dynamic _onIgnored;

        public RuleBookBuilder()
        {
            _ruleStore = ComponentFactory.Instance.Create<IRuleStore>();
        }

        public RuleBookBuilder<TTarget, TComparison> AddRule(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            _ruleStore.UpdateRule<TTarget, TComparison>(ruleDto);

            return this;
        }  
        
        public RuleBookBuilder<TTarget, TComparison> OnSuccess(Action<TTarget, TComparison> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _onSuccess = action;

            return this;
        }

        public RuleBookBuilder<TTarget, TComparison> OnSuccess<TResult>(Func<TTarget, TComparison, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            _onSuccess = func;

            return this;
        }

        public RuleBookBuilder<TTarget, TComparison> OnFailure(Action<TTarget, TComparison> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _onFailure = action;

            return this;
        }

        public RuleBookBuilder<TTarget, TComparison> OnIgnored(Action<TTarget, TComparison> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _onIgnored = action;

            return this;
        }

        public RuleBook<TTarget, TComparison> Build()
        {
            return new RuleBook<TTarget, TComparison>
            {
                OnSuccess = _onSuccess,
                OnFailure = _onFailure,
                OnIgnored = _onIgnored
            };
        }
    }
}
