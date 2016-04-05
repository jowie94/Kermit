using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Parser;

namespace Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            //var globalScope = new GlobalScope();
            //Stream inputStream = Console.OpenStandardInput();
            Repl.Loop();
        }
    }
}
