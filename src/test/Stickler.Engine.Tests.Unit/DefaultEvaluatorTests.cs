using System.Collections.Generic;
using NUnit.Framework;

namespace Stickler.Engine.Tests.Unit
{
    [TestFixture]
    public class DefaultEvaluatorTests
    {
        [Test]
        public void Evaluate_GivenValidRules_ReturnsCorrectResultSet()
        {
            var compiler = new DefaultCompiledRuleStore(new DefaultRuleStore());
            var ruleDtos = new List<RuleDto>
            {
                new RuleDto
                {
                    Name = "Holding NAV within 10 of Fund NAV",
                    RuleExpression = "ensure holding.GetNav() is within 10 of fund.Nav",
                    TargetTypeName = "Holding"
                },
                new RuleDto
                {
                    Name = "Holding NAV is greater than Fund NAV",
                    RuleExpression = "ensure holding.Nav is greaterThan fund.GetNav()",
                    TargetTypeName = "Holding"
                },
                new RuleDto
                {
                    Name = "Holding NAV is equal to Fund NAV",
                    RuleExpression = "ensure holding.Nav is equalTo fund.Nav",
                    TargetTypeName = "Holding"
                }
            };

            foreach (var ruleDto in ruleDtos)
            {
                compiler.AddRule<DefaultInterpreterTests.Holding, DefaultInterpreterTests.Fund>(ruleDto);
            }

            var sut = new DefaultEvaluator(compiler);

            var holding = new DefaultInterpreterTests.Holding
            {
                Nav = 25
            };

            var fund = new DefaultInterpreterTests.Fund
            {
                Nav = 20
            };
            
            var expected = new ResultSet
            {
                Status = ResultStatus.Fail,
                Results = new List<Result>
                {
                    new Result
                    {
                        RuleName = "Holding NAV within 10 of Fund NAV",
                        RuleExpression = "ensure holding.GetNav() is within 10 of fund.Nav",
                        Status = ResultStatus.Pass
                    },
                    new Result
                    {
                        RuleName = "Holding NAV is greater than Fund NAV",
                        RuleExpression = "ensure holding.Nav is greaterThan fund.GetNav()",
                        Status = ResultStatus.Pass
                    },
                    new Result
                    {
                        RuleName = "Holding NAV is equal to Fund NAV",
                        RuleExpression = "ensure holding.Nav is equalTo fund.Nav",
                        Status = ResultStatus.Fail
                    }
                }
            };

            var actual = sut.Evaluate(holding, fund);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
