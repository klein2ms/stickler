using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stickler.Engine
{
    public class DefaultInterpreter : IInterpreter
    {
        public Func<TTarget, TComparison, bool> Interpret<TTarget, TComparison>(RuleDefinition rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            
            if (!string.Equals(typeof(TTarget).Name, rule.TargetTypeName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"The rule expected a target type of {rule.TargetTypeName} but the rule was executed against the target type {typeof(TTarget).Name}.");
            
            var targetParameterExpression = Expression.Parameter(typeof(TTarget));

            var targetProperty = typeof(TTarget).GetProperty(rule.TargetAttribute);

            if (targetProperty == null)
                throw new ArgumentException($"Target does not have a property named: {rule.TargetAttribute}");

            var targetPropertyExpression = Expression.Property(targetParameterExpression, targetProperty);

            if (!rule.RuleConditions.Any())
                throw new ArgumentException("Rule does not contain any rule conditions");

            if (rule.RuleConditions.Count != 1)
                throw new NotImplementedException("Rules do not support multiple rule conditions");

            var ruleCondition = rule.RuleConditions.First();

            if (!string.Equals(typeof(TComparison).Name, ruleCondition.ComparisonTypeName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"The rule expected a comparison type of {ruleCondition.ComparisonTypeName} but the rule was executed against the comparison type {typeof(TComparison).Name}.");
            
            var comparisonParameterExpression = Expression.Parameter(typeof(TComparison));

            var comparisonProperty = typeof(TComparison).GetProperty(ruleCondition.ComparisonAttribute);

            if (comparisonProperty == null)
                throw new ArgumentException($"Comparison does not have a property named: {ruleCondition.ComparisonAttribute}");

            var comparisonPropertyExpression = Expression.Property(comparisonParameterExpression, comparisonProperty);

            ConditionalExpression body = null;

            switch (ruleCondition.Operator)
            {
                case RuleComparatorOperator.GreaterThan:
                    body = Expression.Condition(
                        Expression.GreaterThan(targetPropertyExpression, comparisonPropertyExpression),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                case RuleComparatorOperator.GreaterThanOrEqualTo:
                    body = Expression.Condition(
                        Expression.GreaterThanOrEqual(targetPropertyExpression, comparisonPropertyExpression),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                case RuleComparatorOperator.LessThan:
                    body = Expression.Condition(
                        Expression.LessThan(targetPropertyExpression, comparisonPropertyExpression),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                case RuleComparatorOperator.LessThanOrEqualTo:
                    body = Expression.Condition(
                        Expression.LessThanOrEqual(targetPropertyExpression, comparisonPropertyExpression),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                case RuleComparatorOperator.EqualTo:
                    body = Expression.Condition(
                        Expression.Equal(targetPropertyExpression, comparisonPropertyExpression),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                case RuleComparatorOperator.Within:
                    body = Expression.Condition(
                        Expression.And(
                            Expression.GreaterThanOrEqual(
                                targetPropertyExpression,
                                Expression.Subtract(
                                    comparisonPropertyExpression,
                                    Expression.Constant(decimal.Parse(ruleCondition.Value)))),
                            Expression.LessThanOrEqual(
                                targetPropertyExpression,
                                Expression.Add(
                                    comparisonPropertyExpression,
                                    Expression.Constant(decimal.Parse(ruleCondition.Value))))
                        ),
                        Expression.Constant(true),
                        Expression.Constant(false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleCondition.Operator));
            }
            
            return Expression
                .Lambda<Func<TTarget, TComparison, bool>>(
                    body,
                    targetParameterExpression,
                    comparisonParameterExpression)
                .Compile();
        }
    }
}
