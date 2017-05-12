namespace Stickler.Engine
{
    public class LexiconMatch
    {
        public Lexicon Lexicon { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
    }
}
