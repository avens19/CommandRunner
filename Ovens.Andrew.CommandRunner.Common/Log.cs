using System;
using System.Configuration;
using System.Diagnostics;

namespace Ovens.Andrew.CommandRunner.Common
{
    /// <summary>
    ///     The logging mechanism for the CommandRunner. Uses Windows event logging to avoid file locking issues when multiple
    ///     instances are running
    /// </summary>
    public static class Log
    {
        private const string LogName = "Application";
        private static string _source;
        private static int _id;

        public static void Initialize(string sourceName)
        {
            if(string.IsNullOrWhiteSpace(sourceName))
                throw new ArgumentNullException("LogSourceName");

            _source = sourceName;
            _id = Process.GetCurrentProcess().Id;
        }

        public static void Install()
        {
            EventLog.CreateEventSource(_source, LogName);
            Comment(string.Format("Installed {0}", _source));
        }

        public static void Uninstall()
        {
            EventLog.DeleteEventSource(_source);
        }

        public static void Error(string message)
        {
            EventLog.WriteEntry(_source, message, EventLogEntryType.Error, _id);
        }

        public static void Error(string format, params object[] args)
        {
            EventLog.WriteEntry(_source, string.Format(format, args), EventLogEntryType.Error, _id);
        }

        public static void Warning(string message)
        {
            EventLog.WriteEntry(_source, message, EventLogEntryType.Warning, _id);
        }

        public static void Warning(string format, params object[] args)
        {
            EventLog.WriteEntry(_source, string.Format(format, args), EventLogEntryType.Warning, _id);
        }

        public static void Comment(string message)
        {
            EventLog.WriteEntry(_source, message, EventLogEntryType.Information, _id);
        }

        public static void Comment(string format, params object[] args)
        {
            EventLog.WriteEntry(_source, string.Format(format, args), EventLogEntryType.Information, _id);
        }
    }
}