using System.Collections.Generic;

namespace Stickler.Engine
{
    public interface ILexer
    {
        IEnumerable<RuleToken> Lex(string expression);
    }
}
