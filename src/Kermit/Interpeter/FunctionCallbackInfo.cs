using System.Collections.Generic;
using Interpeter.Types;

namespace Interpeter
{
    public class FunctionCallbackInfo
    {
        private readonly List<KVariable> _parameterList;

        public int Length => _parameterList.Count;

        public InterpreterState InterpreterState { get; }

        public ReturnValue ReturnValue { get; }

        internal FunctionCallbackInfo(List<KVariable> parameters, InterpreterState state)
        {
            _parameterList = parameters;
            InterpreterState = state;
            ReturnValue = new ReturnValue();
        }

        public KVariable this[int id]
        {
            get { return _parameterList[id]; }
            set { _parameterList[id] = value; }
        }
    }
}
