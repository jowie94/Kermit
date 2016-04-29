using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Kermit.Interpeter.Exceptions;
using Kermit.Interpeter.MemorySpaces;
using Kermit.Interpeter.Types;
using Kermit.Parser;
using Kermit.Parser.Exceptions;
using KermitLexer = Kermit.Parser.KermitLexer;
using KermitParser = Kermit.Parser.KermitParser;

namespace Kermit.Interpeter
{
    public class Interpreter : InterpreterState
    {

        #region Internal classes
        private class DummyListener : IInterpreterListener
        {
            public void Write(string msg) {}
            public void Info(string msg) {}
            public void Error(string msg) {}
            public void Error(Exception e) {}

            public void Error(string msg, Exception e) {}
            public void Error(string msg, IToken token) {}
            public string ReadLine() => string.Empty;
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
        private readonly KermitParser _parser;
        private KermitAST _root;
        private MemorySpace _currentSpace;
        private readonly Stack<FunctionSpace> _stack = new Stack<FunctionSpace>();
        private readonly ReturnValue _sharedReturnValue = new ReturnValue();
        #endregion

        public bool ReplMode = false;

        internal override Stack<FunctionSpace> Stack => _stack;

        // ReSharper disable MemberCanBePrivate.Global
        public Interpreter(IInterpreterListener listener) : this(new GlobalScope(), listener)
        {
            ((GlobalScope) GlobalScope).CommitScope();
        }

        public Interpreter(IScope globalScope) : this(globalScope, new DummyListener()) {}

        public Interpreter(IScope globalScope, IInterpreterListener listener)
        {
            if (globalScope == null)
                throw new ArgumentNullException(nameof(globalScope), "Global scope can't be null");
            GlobalScope = globalScope;

            if (listener == null)
                throw new ArgumentNullException(nameof(listener), "Listener can't be null");
            Listener = listener;

            _parser = new KermitParser(null, globalScope) {TreeAdaptor = new KermitAdaptor()};
            //_parser.TraceDestination = Console.Error;

            _currentSpace = Globals;

            AddInternalNativeFunctions();
            AddNativeType("List", typeof(List<object>));
            AddNativeType(typeof(string));
            AddNativeType("Dictionary", typeof(Dictionary<object, object>));
        }

        public void AddNativeFunction(string name, NativeFunction function)
        {
            function.Name = name;
            NativeFunctionSymbol symbol = new NativeFunctionSymbol(name, GlobalScope, function);
            GlobalScope.Define(symbol);
        }

        public void AddNativeType(Type type)
        {
            GlobalScope.Define(new NativeSymbol(type));
        }

        public void AddNativeType(string name, Type type)
        {
            GlobalScope.Define(new NativeSymbol(name, type));
        }

        public void Interpret(ANTLRStringStream input)
        {
            KermitLexer lexer = new KermitLexer(input);
            TokenRewriteStream tokens = new TokenRewriteStream(lexer);
            _parser.TokenStream = tokens;

            AstParserRuleReturnScope<KermitAST, CommonToken> ret;
            try
            {
                ret = _parser.program();
            }
            catch (Exception e) when (e is ParserException || e is PartialStatement)
            {
                ComitableScope g = GlobalScope as ComitableScope;
                g?.RevertScope();
                throw;
            }

            if (_parser.NumberOfSyntaxErrors == 0)
            {
                _root = ret.Tree;

                try
                {
                    Block(_root);
                }
                catch (InterpreterException e)
                {
                    if (e.CallStack != null)
                    {
                        Listener.Error("CallStack:");
                        foreach (string call in e.CallStack)
                            Listener.Error(" - " + call);
                    }
                    Listener.Error(e);
                    Stack.Clear(); // Clear the function stack after an error
                }
            }
            else // We shouldn't reach this condition never
            {
                //throw new InterpreterException($"{_parser.NumberOfSyntaxErrors} syntax errors");
                Listener.Error($"{_parser.NumberOfSyntaxErrors} syntax errors");
            }
        }

        public void Interpret(string input)
        {
            ANTLRStringStream stream = new ANTLRStringStream(input, "<stdin>");
            Interpret(stream);
        }
        // ReSharper restore MemberCanBePrivate.Global

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
                    case KermitParser.RPRINT:
                        ReplPrint(tree);
                        break;
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
                    case KermitParser.ARR:
                        return CreateArray(tree);
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
                    // Accessors
                    case KermitParser.INDEX:
                    case KermitParser.DOT:
                    case KermitParser.ID:
                        return Load(tree);
                    default:
                        throw new InvalidOperationException("Node " + tree.Text + "<" + tree.Type + "> not handled");
                }
            }
            catch (InterpreterException e) when (e.CallStack == null)
            {
                e.CallStack = StackTrace;
                throw;
            }
            catch (Exception e) when (!(e is InterpreterException || e is ReturnValue))
            {
                ThrowHelper.InterpreterException(e.Message, e, StackTrace);
            }

            return null;
        }

        private void ReplPrint(KermitAST tree)
        {
            KermitAST exec = (KermitAST) tree.GetChild(0);
            KObject obj = Execute(exec);
            if (ReplMode && obj != null && !(obj is KVoid))
                Listener.Write(obj.Value.ToString());
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
            _sharedReturnValue.Value = Execute((KermitAST) tree.GetChild(0));
            throw _sharedReturnValue;
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
                    Execute(fSymbol.BlockAst);
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
                ThrowHelper.NameNotExists(fName, StackTrace);
                //Listener.Error($"Function name {fName} is not defined");
                return null;
            }

            int argCount = tree.ChildCount - 1;

            if (!function.IsNative &&
                (function.Value.Arguments == null && argCount > 0 ||
                function.Value.Arguments != null && argCount != function.Value.Arguments.Count))
            {
                ThrowHelper.TypeError($"Function {fName} takes {argCount} arguments", StackTrace);
                //Listener.Error($"Function {fName}: argument list mismatch");
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
            KermitAST objName = (KermitAST) tree.GetChild(0);

            Symbol s = objName.Scope.Resolve(objName.Text);
            if (s is NativeSymbol)
            {
                object[] args = tree.Children.Skip(1).Select(x => Execute((KermitAST) x).Value).ToArray();
                return InstantiateObject(s as NativeSymbol, args);
            }
            ThrowHelper.NameNotExists(objName.Text, StackTrace);
            return null;
        }

        private KObject CreateArray(KermitAST tree)
        {
            KObject[] args = tree.Children.Select(x => Execute((KermitAST) x)).ToArray();
            return new KArray(args);
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
                if (lhs.Type == KermitParser.DOT || lhs.Type == KermitParser.INDEX)
                {
                    FieldAssign(lhs, value);
                }
                else
                {
                    MemorySpace space = GetSpaceWithSymbol(lhs.Text) ?? _currentSpace;
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
            if (tree.Type == KermitParser.INDEX)
                return LoadItem(tree);

            MemorySpace space = GetSpaceWithSymbol(tree.Text);
            if (space != null)
                return space[tree.Text].Value;
            ThrowHelper.NameNotExists(tree.Text, StackTrace);
            //Listener.Error("No such variable " + tree.Text, tree.Token);
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
            string name = a.Is<IComparable>() ? b.GetType().Name : a.GetType().Name;
            ThrowHelper.TypeError($"Type {name} is not comparable");
            return -1;
        }

        private KObject Add(KermitAST tree)
        {
            KObject a = Execute((KermitAST)tree.GetChild(0));
            KObject b = Execute((KermitAST)tree.GetChild(1));

            if (a.IsString || b.IsString)
                return new KString(a.Value + b.Value.ToString());
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

            // We shouldn't reach this fragment
            ThrowHelper.TypeError($"Unsupported arithmetic operation {tree.Text}");
            return null;
        }

        #region Convenience methods
        private MemorySpace GetSpaceWithSymbol(string id)
        {
            // Check if the current scope contains the id (and it is not the global scope)
            if (!ReferenceEquals(_currentSpace, Globals) && _currentSpace.Contains(id))
                return _currentSpace;
            // Check if the top of the stack contains the id (and it is not the current space)
            if (_stack.Count > 0 && !ReferenceEquals(_stack.Peek(), _currentSpace) && _stack.Peek().Contains(id))
                return _stack.Peek();
            return Globals.Contains(id) ? Globals : null;
        }

        private KObject FieldLoad(KermitAST tree)
        {
            KermitAST leftExpr = (KermitAST) tree.GetChild(0);
            KermitAST field = (KermitAST) tree.GetChild(1);
            KObject obj = Execute(leftExpr);
            KObject val;
            string name;

            if (field.Type == KermitParser.INDEX)
                field = (KermitAST) field.GetChild(0);

            if (field.Type == KermitParser.CALL)
            {
                name = field.GetChild(0).Text;
                object[] args = new object[field.ChildCount - 1];
                for (int i = 0; i < args.Length; ++i)
                {
                    KObject ko = Execute((KermitAST) field.GetChild(i + 1));
                    args[i] = obj is KNativeObject ? ko.Value : ko;
                }
                val = obj.CallInnerFunction(name, args);
            }
            else
            {
                name = field.Text;
                val = obj.GetInnerField(name);
            }

            field = (KermitAST) field.Parent;
            if (field.Type == KermitParser.INDEX)
            {
                KermitAST memberTree = (KermitAST)field.GetChild(1);
                val = LoadItem(val, Execute(memberTree));
                if (val == null)
                    ThrowHelper.TypeError($"'{name}' is not enumerable", StackTrace);
            }

            if (val == null)
                ThrowHelper.NoFieldError(obj.Value.GetType().Name, name, StackTrace);
            return val;
        }

        private KObject LoadItem(KObject enumerable, KObject member)
        {
            if (enumerable is KArray)
                return ((KArray) enumerable)[(KInt) member];
            object obj = enumerable.Value;
            Type objType = obj.GetType();
            MethodInfo method;
            if ((method = objType.GetMethod("GetValue", new[] {typeof(int)})) != null ||
                (method = objType.GetMethod("get_Item")) != null ||
                (method = objType.GetMethod("ElementAt")) != null)
                return TypeHelper.ToKObject(method.Invoke(obj, new[] {member.Value}));
            return null;
        }

        private KObject LoadItem(KermitAST tree)
        {
            KermitAST expr = (KermitAST)tree.GetChild(0);
            KermitAST memberTree = (KermitAST)tree.GetChild(1);
            KObject res = LoadItem(Execute(expr), Execute(memberTree));
            if (res == null)
                ThrowHelper.TypeError($"'{expr.Text}' is not enumerable", StackTrace);
            return res;
        }

        private void FieldAssign(KermitAST tree, KObject value)
        {
            KermitAST leftExpr = (KermitAST)tree.GetChild(0);
            KermitAST field = (KermitAST)tree.GetChild(1);
            KObject obj = Execute(leftExpr);
            if (tree.Type == KermitParser.DOT)
            {
                string name = field.Text;
                if (!obj.SetInnerField(name, value))
                    ThrowHelper.NoFieldError(obj.Value.GetType().Name, name, StackTrace);
            }
            else if (tree.Type == KermitParser.INDEX)
            {
                object real = obj.Value;
                Type objType = real.GetType();
                MethodInfo info = objType.GetMethod("set_Item");
                if (info != null)
                    info.Invoke(real, new[] {Execute(field).Value, value.Value});
                else
                    ThrowHelper.AttributeError($"{tree.Text} is not asignable", StackTrace);
            }
        }

        private KObject InstantiateObject(NativeSymbol symbol, object[] arguments)
        {
            Type[] types = arguments.Select(x => x.GetType()).ToArray();
            ConstructorInfo info = symbol.Type.GetConstructor(types);
            if (info != null)
                return TypeHelper.ToKObject(info.Invoke(arguments));
            ThrowHelper.TypeError("There is no constructor with such argument types", StackTrace);
            return null;
        }

        // Load native functions dynamically
        private void AddInternalNativeFunctions()
        {
            foreach (Type t in GetType().Assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof (NativeFunction)))
                {
                    ConstructorInfo ctor = t.GetConstructor(new Type[] {});
                    if (ctor != null)
                    {
                        NativeFunction instance = (NativeFunction) ctor.Invoke(new object[] {});
                        AddNativeFunction(t.Name, instance);
                    }
                }
            }
        }

        #endregion
    }
}
