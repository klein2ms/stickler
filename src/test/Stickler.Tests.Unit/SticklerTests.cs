using System;
using System.Collections.Generic;
using NUnit.Framework;
using Stickler.Engine;

namespace Stickler.Tests.Unit
{
    [TestFixture]
    public class SticklerTests
    {
        [Test]
        public void Evaluate_GivenRuleSetThatIsSuccessful_CallsSuccessAction()
        {
            Stickler.Instance
                .Enforce(new RuleBookBuilder<Holding, Fund>()
                    .AddRule(new RuleDto
                    {
                        Name = "Alert Rule",
                        TargetTypeName = "Holding",
                        RuleExpression = "ensure holding.Nav is >= fund.Nav"
                    })
                    .OnSuccess((target, comparison) =>
                    {
                        Console.WriteLine($"Succeeded for {target} and {comparison}");
                    })
                    .OnFailure((target, comparison) =>
                    {
                        Console.WriteLine($"Failed for {target} and {comparison}");
                    })
                    .Build())
                .Enforce(new RuleBookBuilder<Fund, Holding>()
                    .AddRule(new RuleDto
                    {
                        Name = "Function Rule",
                        TargetTypeName = "Fund",
                        RuleExpression = "ensure fund.GetNav() is > holding.Nav"
                    })
                    .OnSuccess((target, comparison) => target.Nav + comparison.Nav)
                    .Build());
            
            var holding = new Holding { Nav = 25 };
            var fund = new Fund { Nav = 35 };
            
            var resut = Stickler.Instance.Handle<Fund, Holding, decimal>(fund, holding);

            Assert.That(resut, Is.EqualTo(holding.Nav + fund.Nav));
        }
    }

    public class TestRuleStore : IRuleStore
    {
        private readonly IRuleStore _ruleStore;
        public TestRuleStore()
        {
            _ruleStore = new DefaultRuleStore();
        }
        public IEnumerable<RuleDto> GetRules<TTarget, TComparison>()
        {
            return _ruleStore.GetRules<TTarget, TComparison>();
        }

        public RuleDto AddRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            return _ruleStore.AddRule<TTarget, TComparison>(ruleDto);
        }

        public RuleDto UpdateRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            return _ruleStore.UpdateRule<TTarget, TComparison>(ruleDto);
        }

        public void DeleteRule(RuleDto ruleDto)
        {
            _ruleStore.DeleteRule(ruleDto);
        }
    }

    public class Holding
    {
        public decimal Nav { get; set; }
        public decimal GetNav()
        {
            return Nav;
        }
    }

    public class Fund
    {
        public decimal Nav { get; set; }

        public decimal GetNav()
        {
            return Nav;
        }
    }
}
