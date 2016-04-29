using System;
using Antlr.Runtime;

namespace Kermit.Interpeter
{
    public interface IInterpreterListener
    {
        void Write(string msg);
        void Info(string msg);
        void Error(string msg);
        void Error(string msg, Exception e);
        void Error(string msg, IToken token);
        string ReadLine();
    }
}
