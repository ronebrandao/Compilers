using System;
using System.Runtime.Serialization;

namespace CMPUCCompiler.Exceptions
{

    [Serializable]
    public class InstructionBeginningException : Exception
    {
        public InstructionBeginningException() { }
        public InstructionBeginningException(string message) : base(message) { }
        public InstructionBeginningException(string message, Exception inner) : base(message, inner) { }
        protected InstructionBeginningException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
