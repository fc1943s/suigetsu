using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Suigetsu.Core.Common;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Logging
{
    /// <summary>
    ///     Manager for logging instances and configuration.
    /// </summary>
    public static class LoggingManager
    {
        private static readonly string LogLayout = (Testing.IsTestRunning() ? "[Test] " : string.Empty)
                                                   + "[${date}] [${level}] [${logger}] ${message}";

        private static readonly Logger Logger = GetCurrentClassLogger();

        private static bool _configured;

        /// <summary>
        ///     Provides a global logging instance for quick testing purpuses. Use the
        ///     <see cref="GetCurrentClassLogger" /> for production code.
        /// </summary>
        public static Logger GetGlobalLogger() => Logger;

        /// <summary>
        ///     Configures windows event logging (Error level).
        /// </summary>
        [ExcludeFromCodeCoverage]
        private static void ConfigureEventTarget(LoggingConfiguration config)
        {
            if(Environment.UserInteractive)
            {
                return;
            }

            var eventTarget = new EventLogTarget
            {
                Layout = LogLayout
            };

            config.AddTarget("eventlog", eventTarget);

            var eventTargetRule = new LoggingRule("*", LogLevel.Error, eventTarget);
            config.LoggingRules.Add(eventTargetRule);
        }

        /// <summary>
        ///     Configures console logging (Trace level).
        /// </summary>
        private static void ConfigureConsoleTarget(LoggingConfiguration config)
        {
            var consoleTarget = new ColoredConsoleTarget
            {
                Layout = LogLayout
            };

            config.AddTarget("console", consoleTarget);

            var consoleTargetRule = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(consoleTargetRule);
        }

        /// <summary>
        ///     Configures file logging. The log file is located at the [DomainBaseDirectory]\Log (Trace level).
        /// </summary>
        [ExcludeFromCodeCoverage]
        private static void ConfigureFileTarget(LoggingConfiguration config)
        {
            //TODO: code missing
            if(Testing.IsTestRunning() /*|| !Settings.Settings.Get<Settings.Settings>().FileLog*/)
            {
                return;
            }

            var assembly = AssemblyUtils.GetEntryAssembly();

            var fileTarget = new FileTarget
            {
                FileName =
                    Path.Combine
                        (AppDomain.CurrentDomain.BaseDirectory,
                         "Log",
                         $"{DateTime.Now.ToString("yyyy-MM-dd")} - {assembly.GetName().Name}.log"),
                Layout = LogLayout
            };

            config.AddTarget("file", fileTarget);

            var fileTargetRule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(fileTargetRule);

            Trace.WriteLine($"Registering Log file: {fileTarget.FileName}");
        }

        /// <summary>
        ///     Returns a logger instance for the current calling class.
        /// </summary>
        public static Logger GetCurrentClassLogger()
        {
            if(!_configured || Testing.IsTestRunning())
            {
                _configured = true;

                var config = new LoggingConfiguration();

                ConfigureConsoleTarget(config);
                ConfigureEventTarget(config);
                ConfigureFileTarget(config);

                LogManager.Configuration = config;
            }

            var callingClass = AssemblyUtils.GetCallingTypeName(2);
            return LogManager.GetLogger(callingClass);
        }
    }
}
