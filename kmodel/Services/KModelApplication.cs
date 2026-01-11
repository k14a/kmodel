using Microsoft.Extensions.Logging;
using kmodel.Config;

namespace kmodel.Services;

/// <summary>
/// kmodelアプリケーションのメイン処理クラス
/// </summary>
/// <param name="configLoader"></param>
/// <param name="runner"></param>
/// <param name="logger"></param>
public class KModelApplication(
    ConfigLoader configLoader,
    CodeGenRunner runner,
    ILogger<KModelApplication> logger) {

    private readonly ILogger<KModelApplication> _logger = logger;
    private readonly ConfigLoader _configLoader = configLoader;
    private readonly CodeGenRunner _runner = runner;

    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public KModelConfig Config { get; private set; } = default!;

    /// <summary>
    /// アプリケーションの実行
    /// </summary>
    /// <returns></returns>
    public async Task RunAsync() {
        try {
            var baseDirectory = AppContext.BaseDirectory;
            var path = Path.Combine(baseDirectory, "kmodel.config.json");
            this.Config = this._configLoader.Load(path);

            foreach (var gen in this.Config.Generates) {
                this._logger.LogInformation("Processing AppId: {appId}", gen.AppId);
                await this._runner.RunAsync(gen, this.Config.SubDomain);
            }

            foreach (var raw in this.Config.Raws) {
                this._logger.LogInformation("Processing AppId: {appId}", raw.AppId);
                await this._runner.RunRawAsync(raw, this.Config.SubDomain);
            }

            foreach (var diff in this.Config.Diffs) {
                this._logger.LogInformation("Processing AppId: {appId}", diff.AppId);
                await this._runner.RunDiffAsync(diff, this.Config.SubDomain);
            }

        } catch (Exception ex) {
            this._logger.LogError(ex, "An error occurred during execution: {Message}", ex.Message);
            throw;
        }
    }
}