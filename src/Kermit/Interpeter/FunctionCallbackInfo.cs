using System.Collections.Generic;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Callback information for native functions
    /// </summary>
    public class FunctionCallbackInfo
    {
        private readonly List<KLocal> _parameterList;

        /// <summary>
        /// The amount of parameters
        /// </summary>
        public int Length => _parameterList.Count;

        /// <summary>
        /// The state of the interpreter
        /// </summary>
        public InterpreterState InterpreterState { get; }

        /// <summary>
        /// The value returned by the function
        /// </summary>
        public ReturnValue ReturnValue { get; }

        internal FunctionCallbackInfo(List<KLocal> parameters, InterpreterState state)
        {
            _parameterList = parameters;
            InterpreterState = state;
            ReturnValue = new ReturnValue();
        }

        public KLocal this[int id]
        {
            get { return _parameterList[id]; }
            set { _parameterList[id] = value; }
        }
    }
}
