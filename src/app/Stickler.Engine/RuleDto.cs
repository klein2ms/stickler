namespace Stickler.Engine
{
    public class RuleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetTypeName { get; set; }
        public string RuleExpression { get; set; }

        public RuleDto Clone()
        {
            return new RuleDto
            {
                Id = Id,
                Name = Name,
                TargetTypeName = TargetTypeName,
                RuleExpression = RuleExpression
            };
        }
    }
}
