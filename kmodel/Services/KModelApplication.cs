using Microsoft.Extensions.Logging;
using kmodel.Config;
using kmodel.Logging;

namespace kmodel.Services;

/// <summary>
/// kmodelアプリケーションのメイン処理クラス
/// </summary>
public class KModelApplication(
    ConfigLoader configLoader,
    GenerateRunner generateRunner,
    RawRunner rawRunner,
    DiffRunner diffRunner,
    ILogger<KModelApplication> logger) {
    private readonly ILogger<KModelApplication> _logger = logger;
    private readonly ConfigLoader _configLoader = configLoader;

    private readonly GenerateRunner _generateRunner = generateRunner;
    private readonly RawRunner _rawRunner = rawRunner;
    private readonly DiffRunner _diffRunner = diffRunner;

    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public KModelConfig Config { get; private set; } = default!;

    public async Task RunAsync() {
        try {
            StaticLogMessages.MethodStart(this._logger);
            this.LoadConfig();

            await this.ProcessGeneratesAsync();
            await this.ProcessRawsAsync();
            await this.ProcessDiffsAsync();

        } catch (Exception ex) {
            this._logger.LogError(ex, "An error occurred during execution: {Message}", ex.Message);
            throw;
        } finally {
            StaticLogMessages.MethodFinish(this._logger);
        }
    }

    private void LoadConfig() {
        var baseDirectory = AppContext.BaseDirectory;
        var path = Path.Combine(baseDirectory, "kmodel.config.json");
        this.Config = this._configLoader.Load(path);
    }

    private async Task ProcessGeneratesAsync() {
        foreach (var gen in this.Config.Generates) {
            StaticLogMessages.MethodInfo(this._logger, message: $"Processing Generate AppId: {gen.AppId}");
            await this._generateRunner.RunAsync(gen, this.Config.SubDomain);
        }
    }

    private async Task ProcessRawsAsync() {
        foreach (var raw in this.Config.Raws) {
            StaticLogMessages.MethodInfo(this._logger, message: $"Processing Raw AppId: {raw.AppId}");
            await this._rawRunner.RunAsync(raw, this.Config.SubDomain);
        }
    }

    private async Task ProcessDiffsAsync() {
        foreach (var diff in this.Config.Diffs) {
            StaticLogMessages.MethodInfo(this._logger, message: $"Processing Diff AppId: {diff.AppId}");
            await this._diffRunner.RunAsync(diff, this.Config.SubDomain);
        }
    }
}
