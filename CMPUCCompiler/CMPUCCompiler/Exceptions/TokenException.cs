using System;
using System.Runtime.Serialization;

namespace CMPUCCompiler.Exceptions
{

    [Serializable]
    public class TokenException : Exception
    {
        public TokenException() { }
        public TokenException(string message) : base(message) { }
        public TokenException(string message, Exception inner) : base(message, inner) { }
        protected TokenException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
