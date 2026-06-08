using Microsoft.Extensions.Logging;
using kmodel.Logging;

namespace kmodel.Services;

/// <summary>
/// RunnerBase は、GenerateRunner、RawRunner、DiffRunner などの Runner クラスの共通基底クラスです。共通のロギング機能やエラーハンドリングを提供します。
/// 各 Runner クラスは、RunnerBase を継承して、特定の処理を実装します。RunnerBase は、ILogger を使用して、処理の開始、終了、エラーなどの情報をログに記録します。
/// ExecuteAsync メソッドは、処理の開始メッセージをログに記録し、指定されたアクションを実行します。アクションの実行中に例外が発生した場合は、エラーメッセージをログに記録し、例外を再スローします。
/// Info メソッドは、処理の途中で任意の情報をログに記録するためのヘルパーメソッドです。
/// </summary>
/// <typeparam name="TRunner">Runner の型</typeparam>
/// <param name="logger">ロガー</param>
public abstract class RunnerBase<TRunner>(ILogger<TRunner> logger) {
    /// <summary>ロガー</summary>
    protected readonly ILogger<TRunner> _logger = logger;

    /// <summary>
    /// 処理の開始メッセージをログに記録し、指定されたアクションを実行する。アクションの実行中に例外が発生した場合は、エラーメッセージをログに記録し、例外を再スローする。
    /// </summary>
    /// <param name="startMessage">処理の開始メッセージ</param>
    /// <param name="action">実行するアクション</param>
    /// <returns>非同期タスク</returns>
    protected async Task ExecuteAsync(string startMessage, Func<Task> action) {
        StaticLogMessages.MethodStart(this._logger, message: startMessage);

        try {
            await action();
        } catch (Exception ex) {
            StaticLogMessages.MethodError(this._logger, ex, startMessage);
            throw;
        }
    }

    /// <summary>
    /// 処理の途中で任意の情報をログに記録する。
    /// </summary>
    /// <param name="message">ログに記録するメッセージ</param>
    protected void Info(string message) {
        StaticLogMessages.MethodInfo(this._logger, message: message);
    }
}
