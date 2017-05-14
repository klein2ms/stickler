namespace Stickler.Engine
{
    public class RuleCondition
    {
        public RuleObject ComparisonType { get; set; }
        public string ComparisonTypeName { get; set; }
        public string ComparisonAttribute { get; set; }
        public RuleComparatorOperator Operator { get; set; }
        public string Value { get; set; }
        public RuleLogicalOperator LogicalOperator { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as RuleCondition);
        }

        protected bool Equals(RuleCondition other)
        {
            return other != null 
                && ComparisonType == other.ComparisonType 
                && string.Equals(ComparisonTypeName, other.ComparisonTypeName) 
                && string.Equals(ComparisonAttribute, other.ComparisonAttribute) 
                && Operator == other.Operator && string.Equals(Value, other.Value) 
                && LogicalOperator == other.LogicalOperator;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) ComparisonType;
                hashCode = (hashCode*397) ^ (ComparisonTypeName != null ? ComparisonTypeName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ComparisonAttribute != null ? ComparisonAttribute.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Operator;
                hashCode = (hashCode*397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) LogicalOperator;
                return hashCode;
            }
        }
    }
}
