using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Parser;

namespace Terminal
{
    class Repl
    {
        // TODO: Move out when interpreter is ready
        class KermitAdaptor : CommonTreeAdaptor
        {
            public override object Create(IToken payload)
            {
                return new KermitAST(payload);
            }

            public override object DupNode(object treeNode)
            {
                return treeNode == null ? null : Create(((KermitAST)treeNode).Token);
            }

            public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
            {
                return new KermitErrorNode(input, start, stop, e);
            }
        }

        public static void Loop()
        {
            GlobalScope globalScope = new GlobalScope();
            bool exit = false;
            string input = "";
            KermitParser parser = new KermitParser(null, globalScope);
            parser.TreeAdaptor = new KermitAdaptor();
            while (!exit)
            {
                if (input == string.Empty)
                    Console.Write("> ");
                else
                    Console.Write(".. ");
                input += Console.ReadLine() + "\n";
                ANTLRStringStream stream = new ANTLRStringStream(input);
                KermitLexer lexer = new KermitLexer(stream);
                TokenRewriteStream tokens = new TokenRewriteStream(lexer);
                parser.TokenStream = tokens;
                try
                {
                    AstParserRuleReturnScope<KermitAST, CommonToken> result = parser.program();
                    var tree = (CommonTree)result.Tree;
                    DotTreeGenerator gen = new DotTreeGenerator();
                    Console.WriteLine("{0}", gen.ToDot(tree));
                    input = "";
                    globalScope.CommitScope();

                    Console.WriteLine(globalScope.ToString());
                }
                catch (PartialStatement)
                {
                    globalScope.RevertScope();
                }
            }
        }
    }
}
