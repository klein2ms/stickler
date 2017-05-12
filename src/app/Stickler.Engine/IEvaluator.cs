namespace Stickler.Engine
{
    public interface IEvaluator
    {
        ResultSet Evaluate<TTarget, TComparison>(TTarget target, TComparison comparison);
    }
}
