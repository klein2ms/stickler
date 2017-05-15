using System.CodeDom;
using System.Linq;
using NUnit.Framework;

namespace Stickler.Engine.Tests.Unit
{
    [TestFixture]
    public class DefaultInterpreterTests
    {
        [Test]
        public void Interpret_GivenValidRuleDefinition_ReturnsCorrectResult()
        {
            var lexer = new DefaultLexer();

            const string queryText = "ensure holding.Nav is within 10 of fund.Nav";

            var tokens = lexer.Lex(queryText).ToList();

            var parser = new DefaultParser();

            var ruleDefinition = parser.Parse(tokens);

            var sut = new DefaultInterpreter();

            var ruleFunc = sut.Interpret<Holding, Fund>(ruleDefinition);

            var holding = new Holding
            {
                Nav = 5
            };

            var fund = new Fund
            {
                Nav = 6
            };

            var actual = ruleFunc(holding, fund);

            Assert.That(actual, Is.True);
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
}
