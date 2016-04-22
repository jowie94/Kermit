using System.Collections.Generic;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    public class FunctionCallbackInfo
    {
        private readonly List<KLocal> _parameterList;

        public int Length => _parameterList.Count;

        public InterpreterState InterpreterState { get; }

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
