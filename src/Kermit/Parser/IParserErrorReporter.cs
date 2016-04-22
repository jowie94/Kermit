namespace Kermit.Parser
{
    public interface IParserErrorReporter
    {
        void ReportError(string msg);
    }
}
