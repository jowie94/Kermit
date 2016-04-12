using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Interpeter.MemorySpaces;
using Interpeter.Types;
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
        private Stack<FunctionSpace> _stack = new Stack<FunctionSpace>();  
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

        public readonly ReturnValue SharedReturnValue = new ReturnValue();

        public Interpreter(IScope globalScope, IInterpreterListener listener)
        {
            if (globalScope == null)
                throw new ArgumentNullException(nameof(globalScope), "Global scope can't be null");
            _globalScope = globalScope;

            if (listener == null)
                throw new ArgumentNullException(nameof(listener), "Listener can't be null");
            _listener = listener;

            _parser = new KermitParser(null, _globalScope) {TreeAdaptor = new KermitAdaptor()};
            //_parser.TraceDestination = Console.Error;

            _currentSpace = _globals;
        }

        public void Interpret(ANTLRInputStream input)
        {
            // TODO
        }

        public void Interpret(string input)
        {
            ANTLRStringStream stream = new ANTLRStringStream(input, "<stdin>");
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
                //throw new InterpreterException($"{_parser.NumberOfSyntaxErrors} syntax errors"); // TODO: Better exception
                Listener.Error($"{_parser.NumberOfSyntaxErrors} syntax errors");
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

        private KElement Execute(KermitAST tree)
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
                    case KermitParser.IF:
                        IfStatement(tree);
                        break;
                    case KermitParser.WHILE:
                        WhileLoop(tree);
                        break;
                    case KermitParser.FOR:
                        ForLoop(tree);
                        break;
                    case KermitParser.RETURN:
                        Return(tree);
                        break;
                    case KermitParser.CALL:
                        return Call(tree);
                    // Arithmetic operations
                    case KermitParser.ADD:
                        return Add(tree);
                    case KermitParser.SUB:
                    case KermitParser.MUL:
                    case KermitParser.DIV:
                        return Arithmetic(tree);
                    // Logic operations
                    case KermitParser.EQ:
                        return Eq(tree);
                    case KermitParser.NE:
                        return (KBool) !Eq(tree);
                    case KermitParser.LT:
                        return Lt(tree);
                    case KermitParser.BT:
                        return Bt(tree);
                    case KermitParser.LTE:
                        return (KBool) !Bt(tree);
                    case KermitParser.BTE:
                        return (KBool) !Lt(tree);
                    case KermitParser.NOT:
                        return Not(tree);
                    // Keep types at the bottom
                    case KermitParser.TRUE:
                        return KBool.True;
                    case KermitParser.FALSE:
                        return KBool.False;
                    case KermitParser.NUM:
                        return (KNumber) Execute((KermitAST) tree.GetChild(0))*(tree.ChildCount == 2 ? -1 : 1);
                    case KermitParser.INT:
                        return (KInt) int.Parse(tree.Text);
                    case KermitParser.CHAR:
                        return (KChar) tree.Text[1];
                    case KermitParser.FLOAT:
                        return (KFloat) float.Parse(tree.Text);
                    case KermitParser.STRING:
                        return (KString) tree.Text.Substring(1, tree.Text.Length - 2);
                    case KermitParser.ID:
                        return Load(tree);
                    default:
                        throw new InvalidOperationException("Node " + tree.Text + "<" + tree.Type + "> not handled");
                }
            }
            catch (ReturnValue)
            {
                throw;
            }
            catch (Exception e)
            {
                Listener.Error("Problem executing: " + tree.ToStringTree(), e);
            }

            return null;
        }

        private void WhileLoop(KermitAST tree)
        {
            KermitAST condition = (KermitAST) tree.GetChild(0);
            KermitAST code = (KermitAST) tree.GetChild(1);

            ScopeSpace sp = new ScopeSpace("while", _currentSpace);
            MemorySpace save = _currentSpace;
            _currentSpace = sp;
            KBool cond = (KBool) Execute(condition);
            while (cond)
            {
                Execute(code);
                cond = (KBool) Execute(condition);
            }
            _currentSpace = save;
        }

        private void ForLoop(KermitAST tree)
        {
            KermitAST begin = (KermitAST) tree.GetChild(0);
            KermitAST condition = (KermitAST) tree.GetChild(1);
            KermitAST action = (KermitAST) tree.GetChild(2);
            KermitAST code = (KermitAST) tree.GetChild(3);

            ScopeSpace sp = new ScopeSpace("while", _currentSpace);
            MemorySpace save = _currentSpace;
            _currentSpace = sp;
            for (Execute(begin); (KBool) Execute(condition); Execute(action))
                Execute(code);
            _currentSpace = save;
        }

        private void Return(KermitAST tree)
        {
            SharedReturnValue.Value = Execute((KermitAST) tree.GetChild(0));
            throw SharedReturnValue;
        }

        private KElement Call(KermitAST tree)
        {
            string fName = tree.GetChild(0).Text;
            FunctionSymbol fSymbol = (FunctionSymbol) tree.Scope.Resolve(fName);
            if (fSymbol == null)
            {
                Listener.Error($"Function name {fName} is not defined");
                return null;
            }

            FunctionSpace fSpace = new FunctionSpace(fSymbol);
            MemorySpace savedSpace = _currentSpace;
            _currentSpace = fSpace;

            int argCount = tree.ChildCount - 1;

            if (fSymbol.Arguments == null && argCount > 0 ||
                fSymbol.Arguments != null && argCount != fSymbol.Arguments.Count)
            {
                Listener.Error($"Function {fName}: argument list mismatch");
                return null;
            }

            int i = 0;
            foreach (Symbol argSymbol in fSymbol.Arguments.Values)
            {
                VariableSymbol argument = (VariableSymbol) argSymbol;
                KermitAST argumentTree = (KermitAST) tree.GetChild(i++);
                KElement argumentValue = Execute(argumentTree);
                fSpace[argument.Name] = argumentValue;
            }

            KElement result = null;
            _stack.Push(fSpace);
            try
            {
                Execute(fSymbol.BlockAST);
            }
            catch (ReturnValue returnValue)
            {
                result = returnValue.Value;
            }
            _stack.Pop();
            _currentSpace = savedSpace;
            return result;
        }

        private KBool Not(KermitAST tree)
        {
            KElement inner = Execute((KermitAST) tree.GetChild(0));
            return !inner;
        }

        private void IfStatement(KermitAST tree)
        {
            KermitAST condition = (KermitAST) tree.GetChild(0);
            KermitAST code = (KermitAST) tree.GetChild(1);
            KermitAST elseCode = tree.ChildCount == 3 ? (KermitAST) tree.GetChild(3) : null;

            KBool cres = KBool.ToKBool(Execute(condition));
            if (cres)
                Execute(code);
            else if (elseCode != null)
                Execute(elseCode);
        }

        private void Assign(KermitAST tree)
        {
            KermitAST lhs = (KermitAST) tree.GetChild(0);
            KermitAST expr = tree.GetChild(1) as KermitAST;
            KElement value = Execute(expr);

            if (value != null)
            {
                // TODO: check if DOT

                MemorySpace space = GetSpaceWithSymbol(lhs.Text);
                if (space == null) space = _currentSpace;
                space[lhs.Text] = value;
            }
        }

        private KElement Load(KermitAST tree)
        {
            // TODO: check if DOT

            MemorySpace space = GetSpaceWithSymbol(tree.Text);
            if (space != null)
                return space[tree.Text];
            Listener.Error("No such variable " + tree.Text, tree.Token);
            return null;
        }

        private KBool Eq(KermitAST tree)
        {
            KElement a = Execute((KermitAST) tree.GetChild(0));
            KElement b = Execute((KermitAST) tree.GetChild(1));

            return a == b;
        }

        private KBool Lt(KermitAST tree)
        {
            return Compare(tree) < 0;
        }

        private KBool Bt(KermitAST tree)
        {
            return Compare(tree) > 0;
        }

        private int Compare(KermitAST tree)
        {
            KElement a = Execute((KermitAST)tree.GetChild(0));
            KElement b = Execute((KermitAST)tree.GetChild(1));

            if (a is IComparable && b is IComparable)
            {
                return ((IComparable)a).CompareTo(b);
            }
            throw new ArgumentException("Types are not comparable");
        }

        private KElement Add(KermitAST tree)
        {
            KElement a = Execute((KermitAST)tree.GetChild(0));
            KElement b = Execute((KermitAST)tree.GetChild(1));

            if (a.Type == KType.String || b.Type == KType.String)
                return new KString(a.Value.ToString() + b.Value.ToString());
            return Arithmetic(tree);
        }

        private KElement Arithmetic(KermitAST tree)
        {
            KNumber a = (KNumber) Execute((KermitAST) tree.GetChild(0));
            KNumber b = (KNumber) Execute((KermitAST) tree.GetChild(1));
            switch (tree.Type)
            {
                case KermitParser.ADD:
                    return a + b;
                case KermitParser.SUB:
                    return a - b;
                case KermitParser.MUL:
                    return a*b;
                case KermitParser.DIV:
                    return a/b;
            }

            // TODO: Maybe throw error?
            return null;
        }

        #region Convenience methods
        private MemorySpace GetSpaceWithSymbol(string id)
        {
            // Check if the current scope contains the id (and it is not the global scope)
            if (!ReferenceEquals(_currentSpace, _globals) && _currentSpace.Contains(id))
                return _currentSpace;
            // Check if the top of the stack contains the id (and it is not the current space)
            if (_stack.Count > 0 && !ReferenceEquals(_stack.Peek(), _currentSpace) && _stack.Peek().Contains(id))
                return _stack.Peek();
            return _globals.Contains(id) ? _globals : null;
        }

        #endregion
    }
}
