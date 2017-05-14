using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Stickler.Engine.Tests.Unit
{
    [TestFixture]
    public class DefaultParserTests
    {
        [Test]
        public void Parse_GivenValidTokens_ReturnsCorrectResult()
        {
            var lexer = new DefaultLexer();

            const string queryText = "ensure holding.Nav is within 10 of fund.Nav";

            var tokens = lexer.Lex(queryText).ToList();

            var sut = new DefaultParser();

            var expected = new RuleDefinition
            {
                Id = null,
                RuleConditions = new List<RuleCondition>
                {
                    new RuleCondition
                    {
                        ComparisonType = RuleObject.ObjectProperty,
                        ComparisonAttribute = "Nav",
                        ComparisonTypeName = "fund",
                        LogicalOperator = RuleLogicalOperator.None,
                        Operator = RuleComparatorOperator.Within,
                        Value = "10"
                    }
                },
                TargetAttribute = "Nav",
                TargetType = RuleObject.ObjectProperty,
                TargetTypeName = "holding"
            };

            var actual = sut.Parse(tokens);
            
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Parse_GivenTargetObjectPropertyOfMethod_ReturnsCorrectResult()
        {
            var lexer = new DefaultLexer();

            const string queryText = "ensure target.SomeMethod() is within 10 of comparison.SomeProperty";

            var tokens = lexer.Lex(queryText).ToList();

            var sut = new DefaultParser();

            var expected = new RuleDefinition
            {
                Id = null,
                RuleConditions = new List<RuleCondition>
                {
                    new RuleCondition
                    {
                        ComparisonType = RuleObject.ObjectProperty,
                        ComparisonAttribute = "SomeProperty",
                        ComparisonTypeName = "comparison",
                        LogicalOperator = RuleLogicalOperator.None,
                        Operator = RuleComparatorOperator.Within,
                        Value = "10"
                    }
                },
                TargetAttribute = "SomeMethod",
                TargetType = RuleObject.ObjectMethod,
                TargetTypeName = "target"
            };

            var actual = sut.Parse(tokens);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Regex()
        {
            var targetAttribute = "Test()";

            var methodRegex = new Regex("\\w+\\(\\)");

            RuleObject type;

            if (methodRegex.IsMatch(targetAttribute))
            {
                type = RuleObject.ObjectMethod;
                targetAttribute = targetAttribute.Substring(0, targetAttribute.Length - 2);
            }
            else
            {
                type = RuleObject.ObjectProperty;
            }

            Assert.That(type, Is.EqualTo(RuleObject.ObjectMethod));
            Assert.That(targetAttribute, Is.EqualTo("Test"));
        }

        [Test]
        public void Parse_GivenValidTokensWithAnd_ReturnsCorrectResult()
        {
            var lexer = new DefaultLexer();

            const string queryText = "ensure holding.Nav is within 10 of fund.Nav and lessThan 5";

            var tokens = lexer.Lex(queryText).ToList();

            var sut = new DefaultParser();

            var expected = new RuleDefinition
            {
                Id = null,
                RuleConditions = new List<RuleCondition>
                {
                    new RuleCondition
                    {
                        ComparisonType = RuleObject.ObjectProperty,
                        ComparisonAttribute = "Nav",
                        ComparisonTypeName = "fund",
                        LogicalOperator = RuleLogicalOperator.And,
                        Operator = RuleComparatorOperator.Within,
                        Value = "10"
                    },
                    new RuleCondition
                    {
                        ComparisonType = RuleObject.Constant,
                        ComparisonAttribute = null,
                        ComparisonTypeName = "number",
                        LogicalOperator = RuleLogicalOperator.None,
                        Operator = RuleComparatorOperator.LessThan,
                        Value = "5"
                    }
                },
                TargetAttribute = "Nav",
                TargetType = RuleObject.ObjectProperty,
                TargetTypeName = "holding"
            };

            var actual = sut.Parse(tokens);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
