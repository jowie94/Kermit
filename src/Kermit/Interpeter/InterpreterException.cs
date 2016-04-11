﻿using System;
using System.Runtime.Serialization;

namespace Interpeter
{
    [Serializable]
    public class InterpreterException : Exception
    {

        public InterpreterException(string message) : base(message)
        {
        }

        public InterpreterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}