using System;
using System.Collections.Generic;
using JRPG.Core;
using System.IO;

namespace JRPG.Services.Tests
{

    public sealed class TestLog : ILog
    {
        public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

        /// <summary>All emitted log lines (after level filtering).</summary>
        public readonly List<(LogLevel level, string text)> Entries = new();

        /// <summary>Where to mirror logs (e.g., TestContext.Out or TestContext.Progress).</summary>
        private readonly TextWriter _writer;

        public TestLog(TextWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        private bool Allow(LogLevel level) =>
            level >= MinimumLevel && MinimumLevel != LogLevel.None;

        private void Add(LogLevel level, string text)
        {
            if (!Allow(level)) return;

            Entries.Add((level, text));

            _writer.WriteLine(text);
        }

        // ---- ILog implementation ----
        public void Trace(string format, params object[] args) => 
            Add(LogLevel.Trace, "[TRACE] " + (args == null || args.Length == 0 ? format : string.Format(format, args)));

        public void Debug(string format, params object[] args) =>
            Add(LogLevel.Debug, "[DEBUG] " + (args == null || args.Length == 0 ? format : string.Format(format, args)));

        public void Information(string format, params object[] args) => 
            Add(LogLevel.Information, args == null || args.Length == 0 ? format : string.Format(format, args));

        public void Warning(string format, params object[] args) =>
            Add(LogLevel.Warning, "[WARN] " + (args == null || args.Length == 0 ? format : string.Format(format, args)));

        public void Error(string format, params object[] args) =>
            Add(LogLevel.Error, "[ERROR] " + (args == null || args.Length == 0 ? format : string.Format(format, args)));

        public void Critical(string format, params object[] args) =>
            Add(LogLevel.Critical, "[CRITICAL] " + (args == null || args.Length == 0 ? format : string.Format(format, args)));
    }
}