// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Openlabs.Mgcxm.Common;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Openlabs.Mgcxm.Internal;

internal static class LoggerFormatting
{
    internal const int MAX_SINK_CHARS = 8;
    internal const string MESSAGE_TEMPLATE = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u5}({ThreadId}:{ThreadName})] [{SinkName}] {Message:lj}{NewLine}{Exception}";
}

/// <summary>
/// Static class to log messages.
/// </summary>
public static class Logger
{
    static LoggerConfiguration? _loggerConfig = null;
    static ILogger? _loggerImpl = null;

    [Obsolete("This method is no longer used, and has no output.")]
    public static void Trace(string message, params object[] arguments) { }

    public static void Debug(string message, params object[] arguments) => PrintMessage(LogEventLevel.Debug, message, arguments);
    public static void Info(string message, params object[] arguments) => PrintMessage(LogEventLevel.Information, message, arguments);
    public static void Warning(string message, params object[] arguments) => PrintMessage(LogEventLevel.Warning, message, arguments);
    public static void Error(string message, params object[] arguments) => PrintMessage(LogEventLevel.Error, message, arguments);
    public static void Fatal(string message, params object[] arguments) => PrintMessage(LogEventLevel.Fatal, message, arguments);

    public static void Exception(string message, Exception exception) 
        => PrintMessage(
            LogEventLevel.Fatal, 
            $"{message}: ({exception.GetType().GetSafeName()}): {exception.Message}\n {exception.StackTrace}", new object[0]);

    public static void Initialize()
    {
        if (_loggerConfig == null && _loggerImpl == null)
        {
            _loggerConfig = new LoggerConfiguration()
                                .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: LoggerFormatting.MESSAGE_TEMPLATE)
                                .Enrich.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)
                                .Enrich.WithProperty("ThreadName", Thread.CurrentThread.Name ?? "None")
                                .Enrich.WithProperty("SinkName", "Main Log"
                                                                    .PadLeft(LoggerFormatting.MAX_SINK_CHARS, ' ')
                                                                    .Truncate(LoggerFormatting.MAX_SINK_CHARS))
                                .MinimumLevel.Debug();

            _loggerImpl = _loggerConfig.CreateLogger();
        }
    }
    
    private static void PrintMessage(LogEventLevel eventLevel, string message, object[] args)
    {
        if (_loggerConfig == null && _loggerImpl == null) return;

        switch (eventLevel)
        {
            case LogEventLevel.Debug:
                _loggerImpl.Debug(message, args); break;
            case LogEventLevel.Information:
                _loggerImpl.Information(message, args); break;
            case LogEventLevel.Warning:
                _loggerImpl.Warning(message, args); break;
            case LogEventLevel.Error:
                _loggerImpl.Error(message, args); break;
            case LogEventLevel.Fatal:
                _loggerImpl.Fatal(message, args); break;

            default: break;
        }
    }
}

public class LoggerSink
{
    LoggerConfiguration? _loggerConfig = null;
    ILogger? _loggerImpl = null;

    public LoggerSink(string sinkName)
    {
        if (sinkName.Length > LoggerFormatting.MAX_SINK_CHARS)
            throw new ArgumentException(
                string.Format("Cannot have a sink name more than {0} characters.", LoggerFormatting.MAX_SINK_CHARS), 
                nameof(sinkName));
        _sinkName = sinkName;

        Initialize();
    }

    private void Initialize()
    {
        if (_loggerConfig == null && _loggerImpl == null)
        {
            _loggerConfig = new LoggerConfiguration()
                                .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: LoggerFormatting.MESSAGE_TEMPLATE)
                                .Enrich.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)
                                .Enrich.WithProperty("ThreadName", Thread.CurrentThread.Name ?? "None")
                                .Enrich.WithProperty("SinkName", _sinkName
                                                                    .PadLeft(LoggerFormatting.MAX_SINK_CHARS, ' ')
                                                                    .Truncate(LoggerFormatting.MAX_SINK_CHARS))
                                .MinimumLevel.Debug();

            _loggerImpl = _loggerConfig.CreateLogger();
        }
    }


    [Obsolete("This method is no longer used, and has no output.")]
    public void Trace(string message, params object[] arguments) { }

    public void Debug(string message, params object[] arguments) => PrintMessage(LogEventLevel.Debug, message, arguments);
    public void Info(string message, params object[] arguments) => PrintMessage(LogEventLevel.Information, message, arguments);
    public void Warning(string message, params object[] arguments) => PrintMessage(LogEventLevel.Warning, message, arguments);
    public void Error(string message, params object[] arguments) => PrintMessage(LogEventLevel.Error, message, arguments);
    public void Fatal(string message, params object[] arguments) => PrintMessage(LogEventLevel.Fatal, message, arguments);

    public void Exception(string message, Exception exception)
        => PrintMessage(
            LogEventLevel.Fatal,
            $"{message}: ({exception.GetType().GetSafeName()}): {exception.Message}\n {exception.StackTrace}", new object[0]);

    private void PrintMessage(LogEventLevel eventLevel, string message, object[] args)
    {
        if (_loggerConfig == null && _loggerImpl == null) return;

        switch (eventLevel)
        {
            case LogEventLevel.Debug:
                _loggerImpl.Debug(message, args); break;
            case LogEventLevel.Information:
                _loggerImpl.Information(message, args); break;
            case LogEventLevel.Warning:
                _loggerImpl.Warning(message, args); break;
            case LogEventLevel.Error:
                _loggerImpl.Error(message, args); break;
            case LogEventLevel.Fatal:
                _loggerImpl.Fatal(message, args); break;

            default: break;
        }
    }

    public string SinkName => _sinkName;
    private string _sinkName;
}