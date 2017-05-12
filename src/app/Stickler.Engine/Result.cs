namespace Stickler.Engine
{
    public class Result
    {
        public string RuleName { get; set; }
        public string RuleExpression { get; set; }
        public ResultStatus Status { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as Result);
        }

        protected bool Equals(Result other)
        {
            return other != null
                && string.Equals(RuleName, other.RuleName) 
                && string.Equals(RuleExpression, other.RuleExpression) 
                && Status == other.Status;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RuleName != null ? RuleName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RuleExpression != null ? RuleExpression.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Status;
                return hashCode;
            }
        }
    }
}
