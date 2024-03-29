﻿using System;
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
    /// <summary>
    /// Main interpreter logic class
    /// </summary>
    public class Interpreter : InterpreterState
    {

        #region Internal classes
        /// <summary>
        /// Dummy listener used when no Listener is passed
        /// </summary>
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
#endregion

        #region Private fields
        private readonly KermitParser _parser;
        private KermitAST _root;
        private MemorySpace _currentSpace;
        private readonly Stack<FunctionSpace> _stack = new Stack<FunctionSpace>();
        private readonly ReturnValue _sharedReturnValue = new ReturnValue();
        #endregion

        /// <summary>
        /// Activates/Desactivates REPL mode in the interpreter
        /// </summary>
        public bool ReplMode = false;

        /// <summary>
        /// Call stack of the interpreter
        /// </summary>
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

        /// <summary>
        /// Add a native function to the interpreter
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="function">Function object to be called</param>
        public void AddNativeFunction(string name, NativeFunction function)
        {
            function.Name = name;
            NativeFunctionSymbol symbol = new NativeFunctionSymbol(name, GlobalScope, function);
            GlobalScope.Define(symbol);
            CommitableScope g = GlobalScope as CommitableScope;
            g?.CommitScope();
        }

        /// <summary>
        /// Add a native type to the interpreter
        /// </summary>
        /// <param name="type">Type to be added</param>
        public void AddNativeType(Type type)
        {
            AddNativeType(type.Name, type);
        }

        /// <summary>
        /// Add a native type to the interpreter
        /// </summary>
        /// <param name="name">Name for the type</param>
        /// <param name="type">Type to be added</param>
        public void AddNativeType(string name, Type type)
        {
            GlobalScope.Define(new NativeSymbol(name, type));
            CommitableScope g = GlobalScope as CommitableScope;
            g?.CommitScope();
        }

        /// <summary>
        /// Execute the input stream
        /// </summary>
        /// <param name="input">Input stream to be executed</param>
        public void InterpretAntlrStream(ANTLRStringStream input)
        {
            KermitLexer lexer = new KermitLexer(input);
            TokenRewriteStream tokens = new TokenRewriteStream(lexer);
            _parser.TokenStream = tokens;

            AstParserRuleReturnScope<KermitAST, CommonToken> ret;
            try
            {
                ret = _parser.program();
                CommitableScope g = GlobalScope as CommitableScope;
                g?.CommitScope();
            }
            catch (Exception e) when (e is ParserException || e is PartialStatement)
            {
                CommitableScope g = GlobalScope as CommitableScope;
                g?.RevertScope();
                throw;
            }

            if (_parser.NumberOfSyntaxErrors == 0)
            {
                _root = ret.Tree;

                try
                {
                    Block(_root);
                    CommitableScope g = GlobalScope as CommitableScope;
                    g?.CommitScope();
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

        public void InterpretLine(string input)
        {
            ANTLRStringStream stream = new ANTLRStringStream(input + "\n", "<stdin>");
            InterpretAntlrStream(stream);
        }

        /// <summary>
        /// Execute the input string
        /// </summary>
        /// <param name="input">Input string with commands to be executed</param>
        public void InterpretBlock(string input)
        {
            ANTLRStringStream stream = new ANTLRStringStream(input, "<stdin>");
            InterpretAntlrStream(stream);
        }
        // ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        /// Execute a block node
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        private void Block(KermitAST tree)
        {
            if (tree.Type != KermitParser.BLOCK)
                ThrowHelper.InterpreterException("Not a block!: " + tree.ToStringTree());

            IList<ITree> children = tree.Children;
            children?.ToList().ForEach(x => Execute((KermitAST) x));
        }

        /// <summary>
        /// Main interpreter method.
        /// This method distributes all node types to the corresponding functions to be executed
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>An object or null, the result of the execution</returns>
        /// <exception cref="InterpreterException">If an internal error occurs</exception>
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

        /// <summary>
        /// Print the result of the node execution to the console if in REPL mode
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        private void ReplPrint(KermitAST tree)
        {
            KermitAST exec = (KermitAST) tree.GetChild(0);
            KObject obj = Execute(exec);
            if (ReplMode && obj != null && !(obj is KVoid))
                Listener.Write(obj.ToString());
        }

        /// <summary>
        /// Performs a while loop
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        private void WhileLoop(KermitAST tree)
        {
            KermitAST condition = (KermitAST) tree.GetChild(0);
            KermitAST code = (KermitAST) tree.GetChild(1);

            ScopeSpace sp = new ScopeSpace("while", _currentSpace);
            MemorySpace save = _currentSpace;
            try
            {
                _currentSpace = sp;
                KBool cond = Execute(condition).Cast<KBool>();
                while (cond)
                {
                    Execute(code);
                    cond = Execute(condition).Cast<KBool>();
                }
            }
            finally
            {
                _currentSpace = save;
            }
        }

        /// <summary>
        /// Performs a for loop
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        private void ForLoop(KermitAST tree)
        {
            KermitAST begin = (KermitAST) tree.GetChild(0);
            KermitAST condition = (KermitAST) tree.GetChild(1);
            KermitAST action = (KermitAST) tree.GetChild(2);
            KermitAST code = (KermitAST) tree.GetChild(3);

            ScopeSpace sp = new ScopeSpace("while", _currentSpace);
            MemorySpace save = _currentSpace;
            try
            {
                _currentSpace = sp;
                for (Execute(begin); (KBool) Execute(condition); Execute(action))
                    Execute(code);
            }
            finally
            {
                _currentSpace = save;
            }
        }

        /// <summary>
        /// Interrupts the current block execution and sets the return value
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        private void Return(KermitAST tree)
        {
            _sharedReturnValue.Value = Execute((KermitAST) tree.GetChild(0));
            throw _sharedReturnValue;
        }

        /// <summary>
        /// Execute a function inside the interpreter
        /// </summary>
        /// <param name="function">Function to be executed</param>
        /// <param name="parameters">Parameters to be passed to a function</param>
        /// <returns>The result of the function</returns>
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
                    ((NativeFunctionSymbol) fSymbol).NativeFunction.SafeExecute(cInfo);
                    result = cInfo.ReturnValue.Value;
                }
                else
                    Execute(fSymbol.BlockAst);
            }
            catch (ReturnValue returnValue)
            {
                result = returnValue.Value;
            }
            finally
            {
                _currentSpace = savedSpace;
            }
            _stack.Pop();
            return result;
        }

        public override void LoadScript(string path)
        {
            ANTLRFileStream fileStream = new ANTLRFileStream(path);
            try
            {
                InterpretAntlrStream(fileStream);
            }
            catch (PartialStatement e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Call a function
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the function</returns>
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

        /// <summary>
        /// Instance an object
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The instantiated object</returns>
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

        /// <summary>
        /// Creates an array
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The created array</returns>
        private KObject CreateArray(KermitAST tree)
        {
            KObject[] args = tree.Children?.Select(x => Execute((KermitAST) x)).ToArray() ?? new KObject[0];
            return new KArray(args);
        }

        /// <summary>
        /// Negates an expression
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The negated expression</returns>
        private KBool Not(KermitAST tree)
        {
            KObject inner = Execute((KermitAST) tree.GetChild(0));
            return !inner;
        }

        /// <summary>
        /// Executes an if statement
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
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

        /// <summary>
        /// Assign a value to a variable
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
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

        /// <summary>
        /// Loads a variable from memory
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The loaded variable</returns>
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

        /// <summary>
        /// Executes an equality instruction between two values
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the equality</returns>
        private KBool Eq(KermitAST tree)
        {
            KObject a = Execute((KermitAST) tree.GetChild(0));
            KObject b = Execute((KermitAST) tree.GetChild(1));

            return a == b;
        }

        /// <summary>
        /// Executes a LT instruction between two values
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the comparison</returns>
        private KBool Lt(KermitAST tree)
        {
            return Compare(tree) < 0;
        }

        /// <summary>
        /// Executes a BT instruction between two values
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the comparison</returns>
        private KBool Bt(KermitAST tree)
        {
            return Compare(tree) > 0;
        }

        /// <summary>
        /// Compares two values if they are IComparable
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the comparison</returns>
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

        /// <summary>
        /// Adds two values.
        /// - If both are numbers, executes the arithmetic operation
        /// - If one of them is a string, then the two string representations are concatenated
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the execution</returns>
        private KObject Add(KermitAST tree)
        {
            KObject a = Execute((KermitAST)tree.GetChild(0));
            KObject b = Execute((KermitAST)tree.GetChild(1));

            if (a.IsString || b.IsString)
                return new KString(a.Value + b.Value.ToString());
            return Arithmetic(tree);
        }

        /// <summary>
        /// Performs an arithmetic operation over two values
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the execution</returns>
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
        /// <summary>
        /// Get the <see cref="MemorySpace"/> containing the named variable
        /// </summary>
        /// <param name="id">The variable to look for</param>
        /// <returns>The <see cref="MemorySpace"/> containing <paramref name="id"/> or null</returns>
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

        /// <summary>
        /// Load a field of an object
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <returns>The result of the field</returns>
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
                FunctionSpace fSpace = new FunctionSpace(new FunctionSymbol(leftExpr.Text + "." + name, GlobalScope));
                MemorySpace savedSpace = _currentSpace;
                _stack.Push(fSpace);
                object[] args = new object[field.ChildCount - 1];
                for (int i = 0; i < args.Length; ++i)
                {
                    KObject ko = Execute((KermitAST) field.GetChild(i + 1));
                    args[i] = obj is KArray ? ko : ko.Value;
                }
                try
                {
                    _currentSpace = fSpace;
                    val = obj.CallInnerFunction(name, args);
                }
                finally
                {
                    _stack.Pop();
                    _currentSpace = savedSpace;
                }
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

        /// <summary>
        /// Load an item from an enumerable object
        /// </summary>
        /// <param name="enumerable">An enumerable object</param>
        /// <param name="member">The member to search</param>
        /// <returns>The loaded item</returns>
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

        /// <summary>
        /// Load an item from an enumerable object
        /// </summary>
        /// <param name="tree">The tree to be executed</param>
        /// <returns>The loaded object</returns>
        private KObject LoadItem(KermitAST tree)
        {
            KermitAST expr = (KermitAST)tree.GetChild(0);
            KermitAST memberTree = (KermitAST)tree.GetChild(1);
            KObject res = LoadItem(Execute(expr), Execute(memberTree));
            if (res == null)
                ThrowHelper.TypeError($"'{expr.Text}' is not enumerable", StackTrace);
            return res;
        }

        /// <summary>
        /// Assign a value to a field
        /// </summary>
        /// <param name="tree">Tree to be executed</param>
        /// <param name="value">Value to be set</param>
        private void FieldAssign(KermitAST tree, KObject value)
        {
            KermitAST leftExpr = (KermitAST)tree.GetChild(0);
            KermitAST field = (KermitAST)tree.GetChild(1);
            KObject obj = Execute(leftExpr);
            bool isTree = tree.Type == KermitParser.INDEX || field.Type == KermitParser.INDEX;
            if (field.Type == KermitParser.INDEX)
            {
                obj = obj.GetInnerField(field.GetChild(0).Text);
                field = (KermitAST) field.GetChild(1);
            }

            if (isTree)
            {
                object fieldValue = Execute(field).Value;
                if (obj is KArray)
                    ((KArray) obj)[(int) fieldValue] = value;
                else
                {
                    object real = obj.Value;
                    Type objType = real.GetType();
                    MethodInfo info = objType.GetMethod("set_Item");
                    if (info != null)
                        info.Invoke(real, new[] {fieldValue, value.Value});
                    else
                        ThrowHelper.AttributeError($"{leftExpr.Text} is not asignable", StackTrace);
                }
            }
            else if (tree.Type == KermitParser.DOT)
            {
                string name = field.Text;
                if (!obj.SetInnerField(name, value))
                    ThrowHelper.NoFieldError(obj.Value.GetType().Name, name, StackTrace);
            }
        }

        /// <summary>
        /// Instantiate a native object
        /// </summary>
        /// <param name="symbol">Symbol object to be instantiated</param>
        /// <param name="arguments">Arguments to be passed to the constructor</param>
        /// <returns>The instantiated object</returns>
        private KObject InstantiateObject(NativeSymbol symbol, object[] arguments)
        {
            Type[] types = arguments.Select(x => x.GetType()).ToArray();
            ConstructorInfo info = symbol.Type.GetConstructor(types);
            if (info != null)
                return TypeHelper.ToKObject(info.Invoke(arguments));
            ThrowHelper.TypeError("There is no constructor with such argument types", StackTrace);
            return null;
        }

        /// <summary>
        /// Loads native functions inside the assembly dynamially
        /// </summary>
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
