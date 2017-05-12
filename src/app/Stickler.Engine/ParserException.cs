using System;
using System.Runtime.Serialization;

namespace Stickler.Engine
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message)
            : base(message)
        {
        }

        public ParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
