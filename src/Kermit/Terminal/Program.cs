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
