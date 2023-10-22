// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Openlabs.Mgcxm.Common;

namespace Openlabs.Mgcxm.Internal;

/// <summary>
/// Static class to log messages.
/// </summary>
public static class Logger
{
    public static void Trace(string message, LoggerSink sink = null) => PrintMessage(LogLevel.TRACE, message, sink);
    public static void Debug(string message, LoggerSink sink = null) => PrintMessage(LogLevel.DEBUG, message, sink);
    public static void Info(string message, LoggerSink sink = null) => PrintMessage(LogLevel.INFO, message, sink);
    public static void Warning(string message, LoggerSink sink = null) => PrintMessage(LogLevel.WARNING, message, sink);
    public static void Error(string message, LoggerSink sink = null) => PrintMessage(LogLevel.ERROR, message, sink);
    public static void Fatal(string message, LoggerSink sink = null) => PrintMessage(LogLevel.FATAL, message, sink);

    public static void Exception(string message, Exception exception, LoggerSink sink = null) 
        => PrintMessage(
            LogLevel.FATAL, 
            $"{message}: ({exception.GetType().GetSafeName()}): {exception.Message}\n {exception.StackTrace}", 
            sink);

    public static void Initialize(Action<LogMessage>? onMessageMade) { OnLogged += onMessageMade.InvokeSafe; }
    
    private static void PrintMessage(LogLevel level, string message, LoggerSink? sink = null)
    {
        if (level < MgcxmConfiguration.CurrentBootstrapConfiguration.minimumLogLevel)
            return;

        var dateTime = DateTime.Now;
        var previousColor = Console.ForegroundColor;
        var currentColor = _messageColors[level];
        
        string dateStr = string.Format("{0:u}", dateTime).Split(" ")[1].Replace("Z", "");
        string levelStr = level.ToString()
            .PadRight(5, ' ');
        
        string sinkName = (sink != null ? sink.SinkName : "   ").PadRight(6, ' ');
        if (sink == null) sinkName = "Main".PadRight(6, ' ');
        
        string ansiMessageStr = string.Format("[{0}] ", dateStr.Pastel("#33ed1f"));
        ansiMessageStr += string.Format("[{0}] ", sinkName.Pastel("#3e89e6"));
        ansiMessageStr += string.Format("[{0}] {1}", levelStr.Pastel(currentColor), message.Pastel(currentColor));
        
        string nonansiMessageStr = string.Format("[{0}] ", dateStr);
        nonansiMessageStr += string.Format("[{0}] ", sinkName);
        nonansiMessageStr += string.Format("[{0}] {1}", levelStr, message);
        _logMessages.Add(nonansiMessageStr);
        
        OnLogged.InvokeSafe(new LogMessage()
        {
            time = dateTime,
            previousColor = previousColor,
            color = currentColor,
            message = ansiMessageStr,
            sink = sink
        });
    }

    public static void SaveLog(string fileName)
    {
        File.WriteAllLines($"{fileName}.txt", _logMessages);
    }

    private static List<string> _logMessages = new List<string>();

    public static event Action<LogMessage> OnLogged = (_ => { });

    private static Dictionary<LogLevel, ConsoleColor> _messageColors = new()
    {
        [LogLevel.DEBUG] = ConsoleColor.Cyan,
        [LogLevel.TRACE] = ConsoleColor.DarkGray,
        [LogLevel.INFO] = ConsoleColor.White,
        [LogLevel.WARNING] = ConsoleColor.Yellow,
        [LogLevel.ERROR] = ConsoleColor.Red,
        [LogLevel.FATAL] = ConsoleColor.DarkRed
    };
}

public class LogMessage
{
    public DateTime time;
    public ConsoleColor previousColor;
    public ConsoleColor color;
    public string message;
    public LoggerSink? sink;
}

/// <summary>
/// An enumeration of all the possible log level types.
/// </summary>
public enum LogLevel
{
    TRACE,
    DEBUG,
    INFO,
    WARNING,
    ERROR,
    FATAL
}

public class LoggerSink
{
    public LoggerSink(string sinkName)
    {
        if (sinkName.Length > 6)
            throw new ArgumentException("Cannot have a sink name more than 6 characters.", nameof(sinkName));
        _sinkName = sinkName;
    }
    
    public void Trace(string message, params object[] args)
        => Logger.Trace((args.Length > 0 ? string.Format(message, args) : message), this);
    
    public void Debug(string message, params object[] args)
        => Logger.Debug((args.Length > 0 ? string.Format(message, args) : message), this);
    
    public void Info(string message, params object[] args)
        => Logger.Info((args.Length > 0 ? string.Format(message, args) : message), this);

    public void Warning(string message, params object[] args)
        => Logger.Warning((args.Length > 0 ? string.Format(message, args) : message), this);

    public void Error(string message, params object[] args)
        => Logger.Error((args.Length > 0 ? string.Format(message, args) : message), this);
    
    public void Fatal(string message, params object[] args)
        => Logger.Fatal((args.Length > 0 ? string.Format(message, args) : message), this);
    
    public void Exception(string message, Exception ex)
        => Logger.Exception(message, ex, this);
    
    public string SinkName => _sinkName;
    private string _sinkName;
}