using Microsoft.Extensions.Logging;
using kmodel.Logging;

namespace kmodel.Services;

public abstract class RunnerBase<TRunner>(ILogger<TRunner> logger) {
    protected readonly ILogger<TRunner> _logger = logger;

    protected async Task ExecuteAsync(string startMessage, Func<Task> action) {
        StaticLogMessages.MethodStart(this._logger, message: startMessage);

        try {
            await action();
        } catch (Exception ex) {
            StaticLogMessages.MethodError(this._logger, ex, startMessage);
            throw;
        }
    }

    protected void Info(string message) {
        StaticLogMessages.MethodInfo(this._logger, message: message);
    }
}
