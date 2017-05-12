using System.Collections.Generic;

namespace Stickler.Engine
{
    public interface IParser
    {
        RuleDefinition Parse(IList<RuleToken> tokens);
    }
}
