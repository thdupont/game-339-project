using System;
using JRPG.Core;

namespace JRPG.Console
{
    // ===================== Logging =====================

    public sealed class ConsoleLog : ILog
    {
        public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

        private bool Allow(LogLevel level) => level >= MinimumLevel && MinimumLevel != LogLevel.None;

        public void Trace(string format, params object[] args)
        { 
            if (Allow(LogLevel.Trace))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine("[TRACE] " + format);
                else
                    System.Console.WriteLine("[TRACE] " + string.Format(format, args)); 
            }
        }

        public void Debug(string format, params object[] args)
        { 
            if (Allow(LogLevel.Debug))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine("[DEBUG] " + format);
                else
                    System.Console.WriteLine("[DEBUG] " + string.Format(format, args)); 
            }
        }

        public void Information(string format, params object[] args)
        { 
            if (Allow(LogLevel.Information))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine(format);
                else
                    System.Console.WriteLine(format, args); 
            }
        }

        public void Warning(string format, params object[] args)
        { 
            if (Allow(LogLevel.Warning))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine("[WARN] " + format);
                else
                    System.Console.WriteLine("[WARN] " + string.Format(format, args)); 
            }
        }

        public void Error(string format, params object[] args)
        { 
            if (Allow(LogLevel.Error))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine("[ERROR] " + format);
                else
                    System.Console.WriteLine("[ERROR] " + string.Format(format, args)); 
            }
        }

        public void Critical(string format, params object[] args)
        { 
            if (Allow(LogLevel.Critical))
            {
                if (args == null || args.Length == 0)
                    System.Console.WriteLine("[CRITICAL] " + format);
                else
                    System.Console.WriteLine("[CRITICAL] " + string.Format(format, args)); 
            }
        }
    }
}