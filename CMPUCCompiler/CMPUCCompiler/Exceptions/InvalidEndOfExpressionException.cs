using System;
using System.Runtime.Serialization;

namespace CMPUCCompiler.Exceptions
{

    [Serializable]
    public class InvalidEndOfExpressionException : Exception
    {
        public InvalidEndOfExpressionException() { }
        public InvalidEndOfExpressionException(string message) : base(message) { }
        public InvalidEndOfExpressionException(string message, Exception inner) : base(message, inner) { }
        protected InvalidEndOfExpressionException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
