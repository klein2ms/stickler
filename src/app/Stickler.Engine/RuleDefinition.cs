using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class RuleDefinition
    {
        public string Id { get; set; }
        public RuleObject TargetType { get; set; }
        public string TargetTypeName { get; set; }
        public string TargetAttribute { get; set; }
        public IList<RuleCondition> RuleConditions { get; set; }
        public RuleDefinition()
        {
            RuleConditions = new List<RuleCondition>();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RuleDefinition);
        }

        protected bool Equals(RuleDefinition other)
        {
            return other != null
                && string.Equals(Id, other.Id) 
                && TargetType == other.TargetType 
                && string.Equals(TargetTypeName, other.TargetTypeName) 
                && string.Equals(TargetAttribute, other.TargetAttribute) 
                && RuleConditions.SequenceEqual(other.RuleConditions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) TargetType;
                hashCode = (hashCode*397) ^ (TargetTypeName != null ? TargetTypeName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TargetAttribute != null ? TargetAttribute.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RuleConditions != null ? RuleConditions.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
