using System.Linq;
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

            const string queryText = "ensure holding.Nav is within 10 of the average for fund.Nav";

            var tokens = lexer.Lex(queryText).ToList();

            var sut = new DefaultParser();

            var actual = sut.Parse(tokens);
            
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void Parse_GivenValidTokensWithAnd_ReturnsCorrectResult()
        {
            var lexer = new DefaultLexer();

            const string queryText = "ensure holding.Nav is within 10 of the average for fund.Nav and lessThan 5";

            var tokens = lexer.Lex(queryText).ToList();

            var sut = new DefaultParser();

            var actual = sut.Parse(tokens);

            Assert.That(actual, Is.Not.Null);
        }
    }
}
