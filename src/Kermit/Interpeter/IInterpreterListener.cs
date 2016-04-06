using System;
using Antlr.Runtime;

namespace Interpeter
{
    public interface IInterpreterListener
    {
        void Info(string msg);
        void Error(string msg);
        void Error(string msg, Exception e);
        void Error(string msg, IToken token);
    }
}
