using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace kmodel.Logging;

public static class StaticLogMessages {
    #region <<Public methods>>
    public static void MethodStart(ILogger? logger, Exception? ex = null, string? message = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodStartInner(logger, className, methodName, ex);
    }
    public static void MethodFinish(ILogger? logger, Exception? ex = null, string? message = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodFinishInner(logger, className, methodName, ex);
    }
    public static void MethodInfo(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodInfoInner(logger, className, methodName, message, ex);
    }
    public static void MethodWarning(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodWarningInner(logger, className, methodName, message, ex);
    }
    public static void MethodError(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodErrorInner(logger, className, methodName, message, ex);
    }
    public static void MethodDebug(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodDebugInner(logger, className, methodName, message, ex);
    }
    #endregion

    #region <<Private methods>>
    private static readonly Action<ILogger, string, string, Exception?> MethodStartInner =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(1001, nameof(MethodStart)),
            "{Class}.{Method} Start.");
    private static readonly Action<ILogger, string, string, Exception?> MethodFinishInner =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(1002, nameof(MethodFinish)),
            "{Class}.{Method} Finish.");
    private static readonly Action<ILogger, string, string, string, Exception?> MethodInfoInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(1003, nameof(MethodInfo)),
            "{Class}.{Method} {Message}.");
    private static readonly Action<ILogger, string, string, string, Exception?> MethodWarningInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            new EventId(1004, nameof(MethodWarning)),
            "{Class}.{Method} {Message}.");
    private static readonly Action<ILogger, string, string, string, Exception?> MethodErrorInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Error,
            new EventId(1005, nameof(MethodError)),
            "{Class}.{Method} {Message}.");
    private static readonly Action<ILogger, string, string, string, Exception?> MethodDebugInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Debug,
            new EventId(1006, nameof(MethodDebug)),
            "{Class}.{Method} {Message}.");
    #endregion
}
