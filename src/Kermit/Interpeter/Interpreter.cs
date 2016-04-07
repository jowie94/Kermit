using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Parser;

namespace Interpeter
{
    public class Interpreter
    {

        #region Internal classes
        private class DummyListener : IInterpreterListener
        {
            public void Info(string msg) {}
            public void Error(string msg) {}
            public void Error(string msg, Exception e) {}
            public void Error(string msg, IToken token) {}
        }

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
#endregion

        #region Private fields
        private IInterpreterListener _listener;
        private IScope _globalScope;
        private KermitParser _parser;
        private KermitAST _root;
        public MemorySpace _globals = new MemorySpace("globals"); // TODO: Public just for debugging! MUST BE PRIVATE
        private MemorySpace _currentSpace;
        #endregion

        public IInterpreterListener Listener
        {
            get { return _listener; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _listener = value;
            }
        }

        public IScope GlobalScope
        {
            get { return _globalScope; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _globalScope = value;
            }
        }

        public Interpreter(IScope globalScope) : this(globalScope, new DummyListener()) {}

        public Interpreter(IScope globalScope, IInterpreterListener listener)
        {
            if (globalScope == null)
                throw new ArgumentNullException(nameof(globalScope), "Global scope can't be null");
            _globalScope = globalScope;

            if (listener == null)
                throw new ArgumentNullException(nameof(listener), "Listener can't be null");
            _listener = listener;

            _parser = new KermitParser(null, _globalScope) {TreeAdaptor = new KermitAdaptor()};

            _currentSpace = _globals;
        }

        public void Interpret(ANTLRInputStream input)
        {
            // TODO
        }

        public void Interpret(string input)
        {
            ANTLRStringStream stream = new ANTLRStringStream(input, "Terminal");
            KermitLexer lexer = new KermitLexer(stream);
            TokenRewriteStream tokens = new TokenRewriteStream(lexer);
            _parser.TokenStream = tokens;

            var ret = _parser.program();
            if (_parser.NumberOfSyntaxErrors == 0)
            {
                _root = ret.Tree;

                Block(_root);
            }
            else
            {
                throw new SystemException(); // TODO: Better exception
            }
        }

        private void Block(KermitAST tree)
        {
            if (tree.Type != KermitParser.BLOCK)
            {
                Listener.Error("Not a block!: " + tree.ToStringTree());
            }

            IList<ITree> children = tree.Children;
            children?.ToList().ForEach(x => Execute((KermitAST) x));
        }

        private object Execute(KermitAST tree)
        {
            try
            {
                switch (tree.Type)
                {
                    case KermitParser.BLOCK:
                        Block(tree);
                        break;
                    case KermitParser.ASSIGN:
                        Assign(tree);
                        break;
                    case KermitParser.ADD:
                        return Add(tree);
                    case KermitParser.SUB:
                    case KermitParser.MUL:
                    case KermitParser.DIV:
                        return Arithmetic(tree);
                    case KermitParser.INT:
                        return int.Parse(tree.Text);
                    case KermitParser.CHAR:
                        return tree.Text[1];
                    case KermitParser.FLOAT:
                        return float.Parse(tree.Text);
                    case KermitParser.STRING:
                        return tree.Text.Substring(1, tree.Text.Length - 2);
                    case KermitParser.ID:
                        return Load(tree);
                    default:
                        throw new InvalidOperationException("Node " + tree.Text + "<" + tree.Type + "> not handled");
                }
            }
            catch (Exception e)
            {
                Listener.Error("Problem executing: " + tree.ToStringTree(), e);
            }

            return null;
        }

        private void Assign(KermitAST tree)
        {
            KermitAST lhs = (KermitAST) tree.GetChild(0);
            KermitAST expr = tree.GetChild(1) as KermitAST;
            object value = Execute(expr);

            if (value != null)
            {
                // TODO: check if DOT

                MemorySpace space = GetSpaceWithSymbol(lhs.Text);
                if (space == null) space = _currentSpace;
                space[lhs.Text] = value;
            }
        }

        private object Load(KermitAST tree)
        {
            // TODO: check if DOT

            MemorySpace space = GetSpaceWithSymbol(tree.Text);
            if (space != null)
                return space[tree.Text];
            Listener.Error("No such variable " + tree.Text, tree.Token);
            return null;
        }

        private object Add(KermitAST tree)
        {
            object a = Execute((KermitAST)tree.GetChild(0));
            object b = Execute((KermitAST)tree.GetChild(1));

            if (a is string || b is string)
                return a.ToString() + b.ToString();
            return Arithmetic(tree);
        }

        private object Arithmetic(KermitAST tree)
        {
            object a = Execute((KermitAST) tree.GetChild(0));
            object b = Execute((KermitAST) tree.GetChild(1));
            dynamic x = null;
            dynamic y = null;

            if (a is float || b is float)
            {
                x = float.Parse(a.ToString());
                y = float.Parse(b.ToString());
            }
            else if (a is int || b is int)
            {
                x = (int) a;
                y = (int) b;
            }

            if (x != null)
            {
                switch (tree.Type)
                {
                    case KermitParser.ADD:
                        return x + y;
                    case KermitParser.SUB:
                        return x - y;
                    case KermitParser.MUL:
                        return x*y;
                    case KermitParser.DIV:
                        return x/y;
                }
            }

            // TODO: Maybe throw error?
            return 0;
        }

        #region Convenience methods
        private MemorySpace GetSpaceWithSymbol(string id)
        {
            // TODO: Explore stack when functions added
            return _globals[id] != null ? _globals : null;
        }

        #endregion
    }
}
