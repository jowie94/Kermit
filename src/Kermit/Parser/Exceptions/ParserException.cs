using System;
using System.Net;

namespace Kermit.Parser.Exceptions
{
    public class ParserException : Exception
    {
        public string SourceName;
        public int Line;
        public int Position;
        public string InputError;

        public override string Message { get; }

        private static string CreateInputError(int line, int pos, string input)
        {
            string[] str = input.Split('\n');
            string bot = string.Empty;
            if (pos != 0)
                bot = $"\n{new string(' ', pos - 1)} ^";
            line = line == 0 ? 0 : line - 1;
            return $"{str[line]}{bot}";
        }

        private static string GenerateMessage(string sourceName, int line, int position, string input, string message)
        {
            return $"File: {sourceName} at line {line}:{position}\n{CreateInputError(line, position, input)}\n{message}";
        }

        [Obsolete]
        public ParserException(string msg, Exception inner) : base(msg, inner) {}

        public ParserException(string sourceName, int line, int position, string input, string message,
            Exception inner) : base("", inner)
        {
            SourceName = sourceName;
            Line = line;
            Position = position;
            InputError = CreateInputError(line, position, input);
            Message = GenerateMessage(sourceName, line, position, input, message);
        }
    }
}
