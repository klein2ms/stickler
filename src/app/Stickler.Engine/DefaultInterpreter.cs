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

            if (!string.Equals(typeof (TTarget).Name, rule.TargetTypeName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException(
                    $"The rule expected a target type of {rule.TargetTypeName} but the rule was executed against the target type {typeof (TTarget).Name}.");

            var targetParameterExpression = Expression.Parameter(typeof(TTarget));
            var targetExpression = GetExpression<TTarget>(targetParameterExpression, rule.TargetType, rule.TargetAttribute);
            
            if (!rule.RuleConditions.Any())
                throw new ArgumentException("Rule does not contain any rule conditions");

            if (rule.RuleConditions.Count != 1)
                throw new NotImplementedException("Rules do not support multiple rule conditions");

            var ruleCondition = rule.RuleConditions.First();

            if (!string.Equals(typeof (TComparison).Name, ruleCondition.ComparisonTypeName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"The rule expected a comparison type of {ruleCondition.ComparisonTypeName} but the rule was executed against the comparison type {typeof (TComparison).Name}.");

            var comparisonParameterExpression = Expression.Parameter(typeof (TComparison));
            var comparisonExpression = GetExpression<TComparison>(
                comparisonParameterExpression,
                ruleCondition.ComparisonType,
                ruleCondition.ComparisonAttribute);
            
            var body = GetExpressionBody(
                ruleCondition.Operator, 
                ruleCondition.Value, 
                targetExpression, 
                comparisonExpression);

            return Expression.Lambda<Func<TTarget, TComparison, bool>>(
                body,
                targetParameterExpression,
                comparisonParameterExpression)
                .Compile();
        }

        private static Expression GetExpression<T>(
            Expression parameterExpression, 
            RuleObject ruleObject, 
            string ruleAttribute)
        {
            switch (ruleObject)
            {
                case RuleObject.ObjectProperty:
                    var property = typeof (T).GetProperty(ruleAttribute);

                    if (property == null)
                        throw new ArgumentException($"{typeof (T)} does not have a property named: {ruleAttribute}");

                    return Expression.Property(parameterExpression, property);
                case RuleObject.ObjectMethod:
                    var method = typeof(T).GetMethod(ruleAttribute);

                    if (method == null)
                        throw new ArgumentException($"{typeof(T)} does not have a method named: {ruleAttribute}");

                    return Expression.Call(parameterExpression, method);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleObject));
            }
        }
        
        private static ConditionalExpression GetExpressionBody(
            RuleComparatorOperator ruleComparatorOperator, 
            string ruleConditionValue, 
            Expression left, 
            Expression right)
        {
            switch (ruleComparatorOperator)
            {
                case RuleComparatorOperator.GreaterThan:
                    return Expression.Condition(Expression.GreaterThan(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.NotGreaterThan:
                    return Expression.Condition(Expression.Not(Expression.GreaterThan(left, right)),
                        Expression.Constant(true), Expression.Constant(false));
                case RuleComparatorOperator.GreaterThanOrEqualTo:
                    return Expression.Condition(Expression.GreaterThanOrEqual(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.NotGreaterThanOrEqualTo:
                    return Expression.Condition(Expression.Not(Expression.GreaterThanOrEqual(left, right)),
                        Expression.Constant(true), Expression.Constant(false));
                case RuleComparatorOperator.LessThan:
                    return Expression.Condition(Expression.LessThan(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.NotLessThan:
                    return Expression.Condition(Expression.Not(Expression.LessThan(left, right)),
                        Expression.Constant(true), Expression.Constant(false));
                case RuleComparatorOperator.LessThanOrEqualTo:
                    return Expression.Condition(Expression.LessThanOrEqual(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.NotLessThanOrEqualTo:
                    return Expression.Condition(Expression.Not(Expression.LessThanOrEqual(left, right)),
                        Expression.Constant(true), Expression.Constant(false));
                case RuleComparatorOperator.EqualTo:
                    return Expression.Condition(Expression.Equal(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.NotEqualTo:
                    return Expression.Condition(Expression.NotEqual(left, right), Expression.Constant(true),
                        Expression.Constant(false));
                case RuleComparatorOperator.Within:
                    return
                        Expression.Condition(
                            Expression.And(
                                Expression.GreaterThanOrEqual(left,
                                    Expression.Subtract(right, Expression.Constant(decimal.Parse(ruleConditionValue)))),
                                Expression.LessThanOrEqual(left,
                                    Expression.Add(right, Expression.Constant(decimal.Parse(ruleConditionValue))))),
                            Expression.Constant(true), Expression.Constant(false));
                case RuleComparatorOperator.NotWithin:
                    return
                        Expression.Condition(
                            Expression.Not(
                                Expression.And(
                                    Expression.GreaterThanOrEqual(left,
                                        Expression.Subtract(right,
                                            Expression.Constant(decimal.Parse(ruleConditionValue)))),
                                    Expression.LessThanOrEqual(left,
                                        Expression.Add(right, Expression.Constant(decimal.Parse(ruleConditionValue)))))),
                            Expression.Constant(true), Expression.Constant(false));
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleComparatorOperator));
            }
        }
    }
}
