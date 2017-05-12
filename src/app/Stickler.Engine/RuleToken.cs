namespace Stickler.Engine
{
    public class RuleToken
    {
        public Lexicon Lexicon { get; set; }
        public string Value { get; set; }

        public RuleToken(Lexicon lexicon)
            : this(lexicon, string.Empty)
        {
        }

        public RuleToken(Lexicon lexicon, string value)
        {
            Lexicon = lexicon;
            Value = value;
        }

        public RuleToken Clone()
        {
            return new RuleToken(Lexicon, Value);
        }
    }
}
