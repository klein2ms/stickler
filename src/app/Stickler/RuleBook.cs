using Stickler.Engine;

namespace Stickler
{
    public class RuleBook<TTarget, TComparison>
    {
        private readonly IEvaluator _evaluator;

        public dynamic OnSuccess { get; set; }
        public dynamic OnFailure { get; set; }
        public dynamic OnIgnored { get; set; }

        internal RuleBook()
        {
            _evaluator = ComponentFactory.Instance.Create<IEvaluator>();
        }
        
        public ResultSet Evaluate(TTarget target, TComparison comparison)
        {
            return _evaluator.Evaluate(target, comparison);
        }
    }
}
