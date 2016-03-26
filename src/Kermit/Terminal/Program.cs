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
        class KermitAdaptor : CommonTreeAdaptor
        {
            public override object Create(IToken payload)
            {
                return new KermitAST(payload);
            }

            public override object DupNode(object treeNode)
            {
                return treeNode == null ? null : Create(((KermitAST) treeNode).Token);
            }

            public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
            {
                return new KermitErrorNode(input, start, stop, e);
            }
        }

        static void Main(string[] args)
        {
            var globalScope = new GlobalScope();
            Stream inputStream = Console.OpenStandardInput();
            ANTLRInputStream stream = new ANTLRInputStream(inputStream);
            var lexer = new KermitLexer(stream);
            var tokens = new TokenRewriteStream(lexer);
            KermitParser parser = new KermitParser(tokens, globalScope);
            parser.TreeAdaptor = new KermitAdaptor();
            var result = parser.program();
            var tree = (CommonTree)result.Tree;
            DotTreeGenerator gen = new DotTreeGenerator();
            Console.WriteLine("{0}", gen.ToDot(tree));
            Console.WriteLine(globalScope.ToString());
            Console.ReadKey();
        }
    }
}
