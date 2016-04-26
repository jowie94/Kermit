using System;
using System.Runtime.CompilerServices;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Kermit.Interpeter;
using Kermit.Parser;
using Kermit.Parser.Exceptions;
using KermitLexer = Kermit.Parser.KermitLexer;
using KermitParser = Kermit.Parser.KermitParser;

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

        private class SimpleListener : IInterpreterListener
        {
            public void Print(string msg)
            {
                Console.WriteLine(msg);
            }

            public void Info(string msg)
            {
                Console.WriteLine(msg);
            }

            public void Error(string msg)
            {
                Console.Error.WriteLine(msg);
            }

            public void Error(string msg, Exception e)
            {
                Console.Error.WriteLine(msg);
                Console.Error.WriteLine(e.Message);
            }

            public void Error(string msg, IToken token)
            {
                Console.Error.WriteLine(msg);
                Console.Error.WriteLine("Token: " + token.Text);
            }
        }

        public static void Loop()
        {
            bool exit = false;
            string input = "";
            Interpreter interpreter = new Interpreter(new SimpleListener());
            interpreter.ReplMode = true;
            while (!exit)
            {
                if (input == string.Empty)
                {
                    Console.Write(">> ");
                    input = Console.ReadLine() + "\n";
                }
                else
                {
                    string tmp;
                    Console.Write(".. ");
                    while ((tmp = Console.ReadLine()) != "")
                    {
                        input += tmp + "\n";
                        Console.Write(".. ");
                    }
                }
                try
                {
                    interpreter.Interpret(input);
                    input = "";
                }
                catch (PartialStatement) {}
                catch (ParserException e)
                {
                    Console.Error.WriteLine(e.Message);
                    input = "";
                }
            }
        }

        public static void Loop2()
        {
            bool exit = false;
            string input = "";
            KermitParser parser = new KermitParser(null);
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
                    var tree = (CommonTree) result.Tree;
                    DotTreeGenerator gen = new DotTreeGenerator();
                    Console.WriteLine("{0}", gen.ToDot(tree));
                    input = "";

                    //Console.WriteLine(globalScope.ToString());
                }
                catch (PartialStatement)
                {
                }
                catch (ParserException e)
                {
                }
            }
        }
    }
}
