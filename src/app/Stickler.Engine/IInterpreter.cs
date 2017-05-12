using System;

namespace Stickler.Engine
{
    public interface IInterpreter
    {
        Func<TTarget, TComparison, bool> Interpret<TTarget, TComparison>(RuleDefinition rule);
    }
}
