namespace Stickler.Engine
{
    public class RuleCondition
    {
        public RuleObject ComparisonType { get; set; }
        public string ComparisonTypeName { get; set; }
        public string ComparisonAttribute { get; set; }
        public RuleComparatorOperator Operator { get; set; }
        public string Value { get; set; }
        public RuleComputationalOperator Computation { get; set; }
        public RuleLogicalOperator LogicalOperator { get; set; }
    }
}
