namespace Stickler.Engine
{
    public enum Lexicon
    {
        Invalid,

        // value types
        Number,
        String,
        Date,

        // key words
        Ensure,
        Is,

        // comparator objects
        Object,

        // comparator operations
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        NotGreaterThan,
        NotGreaterThanOrEqualTo,
        NotLessThan,
        NotLessThanOrEqualTo,
        NotEqualTo,
        Within,
        Of,
        
        // logical operators
        And,
        Or,

        SequenceTerminator
    }
}
