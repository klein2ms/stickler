using System.Collections.Generic;

namespace Stickler.Engine
{
    public class RuleDefinition
    {
        public string Id { get; set; }
        public RuleObject TargetType { get; set; }
        public string TargetTypeName { get; set; }
        public string TargetAttribute { get; set; }
        public RuleComputationalOperator Operator { get; set; }
        public IList<RuleCondition> RuleConditions { get; set; }
        public RuleDefinition()
        {
            RuleConditions = new List<RuleCondition>();
        }
    }
}
