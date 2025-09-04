namespace JRPG.Core
{
    /// <summary>
    /// Logger interface with standard log levels.
    /// </summary>
    public interface ILog
    {
        void Trace(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Information(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
        void Critical(string format, params object[] args);
    }
}
