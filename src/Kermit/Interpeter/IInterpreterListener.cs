using System;
using Antlr.Runtime;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Input/output functions for the interpreter
    /// </summary>
    public interface IInterpreterListener
    {
        /// <summary>
        /// Write a message
        /// </summary>
        /// <param name="msg">Message to be written</param>
        void Write(string msg);
        /// <summary>
        /// Logs information
        /// </summary>
        /// <param name="msg">Message to be written</param>
        void Info(string msg);
        /// <summary>
        /// Logs error information
        /// </summary>
        /// <param name="msg">Message to be written</param>
        void Error(string msg);
        /// <summary>
        /// Logs error information
        /// </summary>
        /// <param name="e">Exception to be displayed</param>
        void Error(Exception e);
        /// <summary>
        /// Logs error information
        /// </summary>
        /// <param name="msg">Message to be written</param>
        /// <param name="e">Exception of the error</param>
        void Error(string msg, Exception e);
        /// <summary>
        /// Logs error information
        /// </summary>
        /// <param name="msg">Message to be written</param>
        /// <param name="token">Offending token</param>
        void Error(string msg, IToken token);
        /// <summary>
        /// Read a line
        /// </summary>
        /// <returns>The read line</returns>
        string ReadLine();
    }
}
