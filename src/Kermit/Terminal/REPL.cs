using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Interpeter;
using Parser;
using Parser.Exceptions;

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
            GlobalScope globalScope = new GlobalScope();
            bool exit = false;
            string input = "";
            Interpreter interpreter = new Interpreter(globalScope, new SimpleListener());
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
                    globalScope.CommitScope();
                    input = "";
                    Console.WriteLine(interpreter._globals);
                }
                catch (PartialStatement)
                {
                    globalScope.RevertScope();
                }
                catch (ParserException e)
                {
                    Console.Error.WriteLine(e.Message);
                    globalScope.RevertScope();
                    input = "";
                }
            }
        }

        public static void Loop2()
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
                    var tree = (CommonTree) result.Tree;
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
                catch (ParserException e)
                {
                    globalScope.RevertScope();
                }
            }
        }
    }
}
