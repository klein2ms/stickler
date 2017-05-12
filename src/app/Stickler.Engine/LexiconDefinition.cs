using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stickler.Engine
{
    public class LexiconDefinition
    {
        private readonly Regex _regex;
        private readonly Lexicon _returnLexicon;
        private readonly int _precedence;

        public LexiconDefinition(Lexicon returnLexicon, string regex)
            : this(returnLexicon, regex, 1)
        {
        }

        public LexiconDefinition(
            Lexicon returnLexicon,
            string regex,
            int precedence)
        {
            _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnLexicon = returnLexicon;
            _precedence = precedence;
        }

        public IEnumerable<LexiconMatch> Match(string input)
        {
            var matches = _regex.Matches(input);

            for (var i = 0; i < matches.Count; i++)
            {
                yield return new LexiconMatch
                {
                    Lexicon = _returnLexicon,
                    Value = matches[i].Value,
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    Precedence = _precedence
                };
            }
        }
    }
}
