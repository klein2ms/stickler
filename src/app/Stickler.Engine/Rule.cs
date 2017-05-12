using System;

namespace Stickler.Engine
{
    public class Rule<TTarget, TComparison>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public Func<TTarget, TComparison, bool> Validate { get; set; }
    }
}
