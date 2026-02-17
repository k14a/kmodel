using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace kmodel.Logging;

/// <summary>
/// 静的なログメッセージクラス。ILogger を使用して、クラス名、メソッド名、メッセージ、例外などの情報をログに記録するための静的なメソッドを提供します。これらのメソッドは、CallerMemberName、CallerFilePath、CallerLineNumber 属性を使用して、呼び出し元の情報を自動的に取得し、ログに含めることができます。
/// ログレベルは、MethodStart と MethodFinish が Debug、MethodInfo が Information、MethodWarning が Warning、MethodError が Error、MethodDebug が Debug に設定されています。これらのメソッドは、アプリケーション全体で一貫したログメッセージを提供し、コードの重複を減らすために使用されます。
/// 例えば、MethodStart メソッドは、処理の開始をログに記録するために使用されます。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method Start." という形式のメッセージをログに記録します。同様に、MethodFinish メソッドは、処理の終了をログに記録するために使用されます。MethodInfo、MethodWarning、MethodError、MethodDebug メソッドは、任意の情報、警告、エラー、デバッグ情報をログに記録するために使用されます。
/// これらのメソッドは、ILogger を受け取るため、任意のクラスで使用することができます。例えば、GenerateRunner クラスや SchemaConverter クラスなどで、処理の開始、終了、情報、警告、エラー、デバッグ情報をログに記録するために使用されます。これにより、アプリケーション全体で一貫したログメッセージを提供し、コードの重複を減らすことができます。
/// なお、これらのメソッドは、ILogger が null の場合は何も行わないようになっています。これにより、ロガーが提供されていない場合でも、コードの実行に支障をきたすことなく使用することができます。
/// </summary>
public static class StaticLogMessages {
    #region <<Public methods>>
    /// <summary>
    /// 処理の開始メッセージをログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method Start." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodStart(ILogger? logger, Exception? ex = null, string? message = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodStartInner(logger, className, methodName, ex);
    }

    /// <summary>
    /// 処理の終了メッセージをログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method Finish." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodFinish(ILogger? logger, Exception? ex = null, string? message = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodFinishInner(logger, className, methodName, ex);
    }

    /// <summary>
    /// 任意の情報をログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method {Message}." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// 例えば、StaticLogMessages.MethodInfo(logger, message: "Processing started.") と呼び出すと、"Class.Method Processing started." というメッセージがログに記録されます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。ex パラメータも任意で、関連する例外がある場合はそれをログに含めることができます。
    /// methodName、filePath、lineNumber パラメータは、呼び出し元の情報を自動的に取得するためのもので、通常は呼び出し元がこれらのパラメータを指定する必要はありません。これらの情報は、ログに記録されるメッセージに含まれ、デバッグやトラブルシューティングの際に役立ちます。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodInfo(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodInfoInner(logger, className, methodName, message, ex);
    }

    /// <summary>
    /// 任意の警告をログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method {Message}." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// 例えば、StaticLogMessages.MethodWarning(logger, message: "Low disk space.") と呼び出すと、"Class.Method Low disk space." というメッセージがログに記録されます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。ex パラメータも任意で、関連する例外がある場合はそれをログに含めることができます。
    /// methodName、filePath、lineNumber パラメータは、呼び出し元の情報を自動的に取得するためのもので、通常は呼び出し元がこれらのパラメータを指定する必要はありません。これらの情報は、ログに記録されるメッセージに含まれ、デバッグやトラブルシューティングの際に役立ちます。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodWarning(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodWarningInner(logger, className, methodName, message, ex);
    }

    /// <summary>
    /// 任意のエラーをログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method {Message}." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// 例えば、StaticLogMessages.MethodError(logger, ex: ex, message: "An error occurred.") と呼び出すと、"Class.Method An error occurred." というメッセージがエラーレベルでログに記録され、関連する例外 ex の情報もログに含まれます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。ex パラメータも任意で、関連する例外がある場合はそれをログに含めることができます。
    /// methodName、filePath、lineNumber パラメータは、呼び出し元の情報を自動的に取得するためのもので、通常は呼び出し元がこれらのパラメータを指定する必要はありません。これらの情報は、ログに記録されるメッセージに含まれ、デバッグやトラブルシューティングの際に役立ちます。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodError(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodErrorInner(logger, className, methodName, message, ex);
    }

    /// <summary>
    /// 任意のデバッグ情報をログに記録する。呼び出し元のクラス名とメソッド名を自動的に取得し、"Class.Method {Message}." という形式のメッセージをログに記録します。ILogger が null の場合は何も行いません。
    /// 例えば、StaticLogMessages.MethodDebug(logger, message: "Variable x value: 42.") と呼び出すと、"Class.Method Variable x value: 42." というメッセージがデバッグレベルでログに記録されます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。ex パラメータも任意で、関連する例外がある場合はそれをログに含めることができます。
    /// methodName、filePath、lineNumber パラメータは、呼び出し元の情報を自動的に取得するためのもので、通常は呼び出し元がこれらのパラメータを指定する必要はありません。これらの情報は、ログに記録されるメッセージに含まれ、デバッグやトラブルシューティングの際に役立ちます。
    /// </summary>
    /// <param name="logger">ログを記録する ILogger インスタンス</param>
    /// <param name="ex">関連する例外（任意）</param>
    /// <param name="message">追加のメッセージ（任意）</param>
    /// <param name="methodName">呼び出し元のメソッド名（自動取得）</param>
    /// <param name="filePath">呼び出し元のファイルパス（自動取得）</param>
    /// <param name="lineNumber">呼び出し元の行番号（自動取得）</param>
    public static void MethodDebug(ILogger? logger, Exception? ex = null, string message = "", [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        if (logger == null) { return; }
        var className = Path.GetFileNameWithoutExtension(filePath);
        methodName = lineNumber > 0 ? $"{methodName}[{lineNumber}]" : methodName;
        MethodDebugInner(logger, className, methodName, message, ex);
    }
    #endregion

    #region <<Private methods>>
    /// <summary>
    /// ILoggerMessage を使用して、処理の開始、終了、情報、警告、エラー、デバッグメッセージを定義します。これらのメッセージは、クラス名、メソッド名、メッセージ、例外などの情報を含む形式で定義されており、ログに記録される際に使用されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、MethodStartInner は、"Class.Method Start." という形式のメッセージを定義しています。MethodInfoInner は、"Class.Method {Message}." という形式のメッセージを定義しています。これらのメッセージは、StaticLogMessages のパブリックなメソッドから呼び出される際に使用されます。
    /// なお、これらの ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    /// <remarks>
    /// ILoggerMessage を使用してログメッセージを定義することで、ログのパフォーマンスが向上し、コードの重複が減少します。ILoggerMessage は、ログレベルやイベントIDなどの情報を含む形式で定義されており、ログに記録される際に使用されます。これらの ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。
    /// </remarks>
    private static readonly Action<ILogger, string, string, Exception?> MethodStartInner =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(1001, nameof(MethodStart)),
            "{Class}.{Method} Start.");

    /// <summary>
    /// 処理の終了メッセージをログに記録するための ILoggerMessage を定義します。"{Class}.{Method} Finish." という形式のメッセージを定義しており、クラス名とメソッド名を含む形式でログに記録されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、StaticLogMessages.MethodFinish(logger) と呼び出すと、"Class.Method Finish." というメッセージがデバッグレベルでログに記録されます。クラス名とメソッド名は、呼び出し元の情報を自動的に取得してログに含めることができます。
    /// なお、この ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    /// <remarks>
    /// ILoggerMessage を使用してログメッセージを定義することで、ログのパフォーマンスが向上し、コードの重複が減少します。ILoggerMessage は、ログレベルやイベントIDなどの情報を含む形式で定義されており、ログに記録される際に使用されます。これらの ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。
    /// </remarks>
    private static readonly Action<ILogger, string, string, Exception?> MethodFinishInner =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(1002, nameof(MethodFinish)),
            "{Class}.{Method} Finish.");

    /// <summary>
    /// 任意の情報をログに記録するための ILoggerMessage を定義します。"{Class}.{Method} {Message}." という形式のメッセージを定義しており、クラス名、メソッド名、追加のメッセージを含む形式でログに記録されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、StaticLogMessages.MethodInfo(logger, message: "Processing started.") と呼び出すと、"Class.Method Processing started." というメッセージが情報レベルでログに記録されます。クラス名とメソッド名は、呼び出し元の情報を自動的に取得してログに含めることができます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。
    /// なお、この ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    private static readonly Action<ILogger, string, string, string, Exception?> MethodInfoInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(1003, nameof(MethodInfo)),
            "{Class}.{Method} {Message}.");

    /// <summary>
    /// 任意の警告をログに記録するための ILoggerMessage を定義します。"{Class}.{Method} {Message}." という形式のメッセージを定義しており、クラス名、メソッド名、追加のメッセージを含む形式でログに記録されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、StaticLogMessages.MethodWarning(logger, message: "Low disk space.") と呼び出すと、"Class.Method Low disk space." というメッセージが警告レベルでログに記録されます。クラス名とメソッド名は、呼び出し元の情報を自動的に取得してログに含めることができます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。
    /// なお、この ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    private static readonly Action<ILogger, string, string, string, Exception?> MethodWarningInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            new EventId(1004, nameof(MethodWarning)),
            "{Class}.{Method} {Message}.");

    /// <summary>
    /// 任意のエラーをログに記録するための ILoggerMessage を定義します。"{Class}.{Method} {Message}." という形式のメッセージを定義しており、クラス名、メソッド名、追加のメッセージを含む形式でログに記録されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、StaticLogMessages.MethodError(logger, ex: ex, message: "An error occurred.") と呼び出すと、"Class.Method An error occurred." というメッセージがエラーレベルでログに記録され、関連する例外 ex の情報もログに含まれます。クラス名とメソッド名は、呼び出し元の情報を自動的に取得してログに含めることができます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。
    /// なお、この ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    private static readonly Action<ILogger, string, string, string, Exception?> MethodErrorInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Error,
            new EventId(1005, nameof(MethodError)),
            "{Class}.{Method} {Message}.");

    /// <summary>
    /// 任意のデバッグ情報をログに記録するための ILoggerMessage を定義します。"{Class}.{Method} {Message}." という形式のメッセージを定義しており、クラス名、メソッド名、追加のメッセージを含む形式でログに記録されます。ILoggerMessage を使用することで、ログメッセージのパフォーマンスが向上し、コードの重複が減少します。
    /// 例えば、StaticLogMessages.MethodDebug(logger, message: "Variable x value: 42.") と呼び出すと、"Class.Method Variable x value: 42." というメッセージがデバッグレベルでログに記録されます。クラス名とメソッド名は、呼び出し元の情報を自動的に取得してログに含めることができます。message パラメータは任意で、必要に応じて追加の情報をログに含めることができます。
    /// なお、この ILoggerMessage は静的に定義されており、アプリケーション全体で共有されます。これにより、一貫したログメッセージが提供され、コードの重複が減少します。
    /// </summary>
    private static readonly Action<ILogger, string, string, string, Exception?> MethodDebugInner =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Debug,
            new EventId(1006, nameof(MethodDebug)),
            "{Class}.{Method} {Message}.");
    #endregion
}
