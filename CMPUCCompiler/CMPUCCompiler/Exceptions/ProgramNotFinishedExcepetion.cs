using System;
using System.Runtime.Serialization;

namespace CMPUCCompiler.Exceptions
{

    [Serializable]
    public class ProgramNotFinishedExcepetion : Exception
    {
        public ProgramNotFinishedExcepetion() { }
        public ProgramNotFinishedExcepetion(string message) : base(message) { }
        public ProgramNotFinishedExcepetion(string message, Exception inner) : base(message, inner) { }
        protected ProgramNotFinishedExcepetion(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
