﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Interpeter.MemorySpaces;
using Interpeter.Types;
using Parser;

namespace Interpeter
{
    public class Interpreter : InterpreterState
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
        private KermitParser _parser;
        private KermitAST _root;
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

        public Interpreter(IScope globalScope) : this(globalScope, new DummyListener()) {}

        public readonly ReturnValue SharedReturnValue = new ReturnValue();

        public Interpreter(IScope globalScope, IInterpreterListener listener)
        {
            if (globalScope == null)
                throw new ArgumentNullException(nameof(globalScope), "Global scope can't be null");
            GlobalScope = globalScope;

            if (listener == null)
                throw new ArgumentNullException(nameof(listener), "Listener can't be null");
            _listener = listener;

            _parser = new KermitParser(null, globalScope) {TreeAdaptor = new KermitAdaptor()};
            //_parser.TraceDestination = Console.Error;

            _currentSpace = _globals;

            AddInternalNativeFunctions();
        }

        public void AddNativeFunction(string name, NativeFunction function)
        {
            function.Name = name;
            NativeFunctionSymbol symbol = new NativeFunctionSymbol(name, GlobalScope, function);
            GlobalScope.Define(symbol);
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

        private KObject Execute(KermitAST tree)
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
                    case KermitParser.NEW:
                        return Instance(tree);
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
                    case KermitParser.DOT:
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
                //_stack.Clear();
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

        public override KObject CallFunction(KFunction function, List<KLocal> parameters)
        {
            FunctionSymbol fSymbol = function.Value;

            FunctionSpace fSpace = new FunctionSpace(fSymbol);
            MemorySpace savedSpace = _currentSpace;
            _currentSpace = fSpace;

            if (!function.IsNative)
                parameters.ForEach(x => fSpace[x.Name] = x);

            KObject result = null;
            _stack.Push(fSpace);
            try
            {
                if (function.IsNative)
                {
                    FunctionCallbackInfo cInfo = new FunctionCallbackInfo(parameters, this);
                    ((NativeFunctionSymbol)fSymbol).NativeFunction.SafeExecute(cInfo);
                    result = cInfo.ReturnValue.Value;
                }
                else
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

        private KObject Call(KermitAST tree)
        {
            string fName = tree.GetChild(0).Text;
            KFunction function = GetFunction(fName);

            if (function == null)
            {
                Listener.Error($"Function name {fName} is not defined");
                return null;
            }

            int argCount = tree.ChildCount - 1;

            if (!function.IsNative &&
                (function.Value.Arguments == null && argCount > 0 ||
                function.Value.Arguments != null && argCount != function.Value.Arguments.Count))
            {
                Listener.Error($"Function {fName}: argument list mismatch");
                return null;
            }

            List<KLocal> param = new List<KLocal>(argCount);
            var arguments = function.Value.Arguments.Values.GetEnumerator();
            for (int i = 0; i < argCount; ++i)
            {
                arguments.MoveNext();
                string name = function.IsNative ? "" : arguments.Current.Name;
                KermitAST argumentTree = (KermitAST) tree.GetChild(i + 1);
                KLocal var;
                if (argumentTree.Type == KermitParser.REF)
                    var = new KGlobal(name, argumentTree.GetChild(0).Text, _currentSpace);
                else
                {
                    KObject argumentValue = Execute(argumentTree);
                    var = new KLocal(name, argumentValue);
                }
                param.Add(var);
            }

            return CallFunction(function, param);
        }

        private KObject Instance(KermitAST tree)
        {
            string objName = tree.GetChild(0).Text;

            throw new NotImplementedException("Instances are currently not supported");
        }

        private KBool Not(KermitAST tree)
        {
            KObject inner = Execute((KermitAST) tree.GetChild(0));
            return !inner;
        }

        private void IfStatement(KermitAST tree)
        {
            KermitAST condition = (KermitAST) tree.GetChild(0);
            KermitAST code = (KermitAST) tree.GetChild(1);
            KermitAST elseCode = tree.ChildCount == 3 ? (KermitAST) tree.GetChild(2) : null;

            KBool cres = TypeHelper.ToBool(Execute(condition));
            if (cres)
                Execute(code);
            else if (elseCode != null)
                Execute(elseCode);
        }

        private void Assign(KermitAST tree)
        {
            KermitAST lhs = (KermitAST) tree.GetChild(0);
            KermitAST expr = tree.GetChild(1) as KermitAST;
            KObject value = Execute(expr);

            if (value != null && !value.IsVoid)
            {
                if (lhs.Type == KermitParser.DOT)
                {
                    FieldAssign(lhs, value);
                }
                else
                {
                    MemorySpace space = GetSpaceWithSymbol(lhs.Text);
                    if (space == null) space = _currentSpace;
                    KLocal var = space[lhs.Text];
                    if (var == null)
                        space[lhs.Text] = new KLocal(lhs.Text, value);
                    else
                        var.Value = value;
                }
            }
        }

        private KObject Load(KermitAST tree)
        {
            if (tree.Type == KermitParser.DOT)
                return FieldLoad(tree);

            MemorySpace space = GetSpaceWithSymbol(tree.Text);
            if (space != null)
                return space[tree.Text].Value;
            Listener.Error("No such variable " + tree.Text, tree.Token); // TODO: Should throw exception
            return null;
        }

        private KBool Eq(KermitAST tree)
        {
            KObject a = Execute((KermitAST) tree.GetChild(0));
            KObject b = Execute((KermitAST) tree.GetChild(1));

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
            KObject a = Execute((KermitAST)tree.GetChild(0));
            KObject b = Execute((KermitAST)tree.GetChild(1));

            if (a.Is<IComparable>() && b.Is<IComparable>())
            {
                return ((IComparable)a).CompareTo(b);
            }
            throw new ArgumentException("Types are not comparable");
        }

        private KObject Add(KermitAST tree)
        {
            KObject a = Execute((KermitAST)tree.GetChild(0));
            KObject b = Execute((KermitAST)tree.GetChild(1));

            if (a.IsString || b.IsString)
                return new KString(a.Value.ToString() + b.Value.ToString());
            return Arithmetic(tree);
        }

        private KObject Arithmetic(KermitAST tree)
        {
            KNumber a = Execute((KermitAST) tree.GetChild(0)).Cast<KNumber>();
            KNumber b = Execute((KermitAST) tree.GetChild(1)).Cast<KNumber>();
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

        private KObject FieldLoad(KermitAST tree)
        {
            KermitAST leftExpr = (KermitAST) tree.GetChild(0);
            KermitAST field = (KermitAST) tree.GetChild(1);
            KObject obj = Execute(leftExpr);
            KObject val;
            string name;

            if (field.Type == KermitParser.CALL)
            {
                name = field.GetChild(0).Text;
                object[] args = new object[field.ChildCount - 1];
                for (int i = 0; i < args.Length; ++i)
                    args[i] = Execute((KermitAST) field.GetChild(i + 1)).Value;
                val = obj.CallInnerFunction(name, args);
            }
            else
            {
                name = field.Text;
                val = obj.GetInnerField(name);
            }

            if (val == null)
                throw new InterpreterException($"Type {obj.Value.GetType().Name} has no field called {name}");
            return val;
        }

        private void FieldAssign(KermitAST tree, KObject value)
        {
            KermitAST leftExpr = (KermitAST)tree.GetChild(0);
            KermitAST field = (KermitAST)tree.GetChild(1);
            KObject obj = Execute(leftExpr);
            string name = field.Text;

            if (!obj.SetInnerField(name, value))
                throw new InterpreterException($"Type {obj.Value.GetType().Name} has no field called {name}");
        }

        // Load native functions dynamically
        private void AddInternalNativeFunctions()
        {
            foreach (Type t in GetType().Assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof (NativeFunction)))
                {
                    ConstructorInfo ctor = t.GetConstructor(new Type[] {});
                    NativeFunction instance = (NativeFunction) ctor.Invoke(new object[] {});
                    AddNativeFunction(t.Name, instance);
                }
            }
        }

        #endregion
    }
}
