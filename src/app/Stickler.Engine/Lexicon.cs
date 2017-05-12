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
        Within,
        Of,
        The,
        For,

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

        // computational operators
        Average,
        Sum,

        // logical operators
        And,
        Or,

        SequenceTerminator
    }
}
