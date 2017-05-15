using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class DefaultLexer : ILexer
    {
        private readonly List<LexiconDefinition> _lexiconDefinitions;

        public DefaultLexer()
        {
            _lexiconDefinitions = new List<LexiconDefinition>
            {
                new LexiconDefinition(Lexicon.And, "and"),
                new LexiconDefinition(Lexicon.Date, "(?:(?:(?:0?[13578]|1[02])(\\/|-|\\.)31)\\1|(?:(?:0?[1,3-9]|1[0-2])(\\/|-|\\.)(?:29|30)\\2))(?:(?:1[6-9]|[2-9]\\d)?\\d{2})|^(?:0?2(\\/|-|\\.)29\\3(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))|^(?:(?:0?[1-9])|(?:1[0-2]))(\\/|-|\\.)(?:0?[1-9]|1\\d|2[0-8])\\4(?:(?:1[6-9]|[2-9]\\d)?\\d{2})", 1),
                new LexiconDefinition(Lexicon.Ensure, "ensure"),
                new LexiconDefinition(Lexicon.EqualTo, "(=|equalTo)"),
                new LexiconDefinition(Lexicon.GreaterThan, "(>|greaterthan)"),
                new LexiconDefinition(Lexicon.GreaterThanOrEqualTo, "(>=|greaterthanorequalto)"),
                new LexiconDefinition(Lexicon.Is, "is"),
                new LexiconDefinition(Lexicon.Invalid, "\\S+", 3),
                new LexiconDefinition(Lexicon.LessThan, "(<|lessthan)"),
                new LexiconDefinition(Lexicon.LessThanOrEqualTo, "(<=|lessthanorequalto)"),
                new LexiconDefinition(Lexicon.Number, "\\d+(\\.\\d+)?", 2),
                new LexiconDefinition(Lexicon.NotEqualTo, "(!=|not\\sequalTo)"),
                new LexiconDefinition(Lexicon.NotGreaterThan, "(!>|not\\sgreaterthan)"),
                new LexiconDefinition(Lexicon.NotGreaterThanOrEqualTo, "(!>=|not\\sgreaterthanorequalto)"),
                new LexiconDefinition(Lexicon.NotLessThan, "(!<|not\\slessthan)"),
                new LexiconDefinition(Lexicon.NotLessThanOrEqualTo, "(!<=|not\\slessthanorequalto)"),
                new LexiconDefinition(Lexicon.Object, "[a-zA-Z_]\\w*\\.[a-zA-Z_]\\w*(\\(\\))?"),
                new LexiconDefinition(Lexicon.Of, "of"),
                new LexiconDefinition(Lexicon.Or, "or"),
                new LexiconDefinition(Lexicon.String, "'\\w*'"),
                new LexiconDefinition(Lexicon.Within, "within")
            };
        }

        public IEnumerable<RuleToken> Lex(string expression)
        {
            var matches = FindMatches(expression);

            var groupedByIndex = matches
                .GroupBy(t => t.StartIndex)
                .OrderBy(g => g.Key)
                .ToList();

            LexiconMatch lastMatch = null;

            foreach (var group in groupedByIndex)
            {
                var bestMatch = group
                    .OrderBy(t => t.Precedence)
                    .First();

                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;
                
                if (bestMatch.Lexicon == Lexicon.String)
                    bestMatch.Value = bestMatch.Value.Trim('\'');

                yield return new RuleToken(bestMatch.Lexicon, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new RuleToken(Lexicon.SequenceTerminator, string.Empty);
        }

        private IEnumerable<LexiconMatch> FindMatches(string expression)
        {
            var matches = new List<LexiconMatch>();

            foreach (var lexiconDefinition in _lexiconDefinitions)
            {
                matches.AddRange(lexiconDefinition.Match(expression));
            }

            return matches;
        }
    }
}
