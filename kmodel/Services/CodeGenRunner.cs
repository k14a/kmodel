using Microsoft.Extensions.Logging;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using System.Text.Json;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Interfaces;

namespace kmodel.Services;

/// <summary>
/// コード生成実行クラス
/// </summary>
/// <param name="fileWriter"></param>
/// <param name="schemaConverter"></param>
/// <param name="schemaProvider"></param>
/// <param name="codeEmitterFactory"></param>
/// <param name="logger"></param>
public class CodeGenRunner(
    IFileWriter fileWriter,
    ISchemaConverter schemaConverter,
    ISchemaProvider schemaProvider,
    ICodeEmitterFactory codeEmitterFactory,
    ILogger<CodeGenRunner> logger) {
    private readonly IFileWriter _fileWriter = fileWriter;
    private readonly ISchemaConverter _schemaConverter = schemaConverter;
    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory = codeEmitterFactory;
    private readonly ILogger<CodeGenRunner> _logger = logger;

    /// <summary>
    /// コード生成の実行
    /// </summary>
    /// <param name="options"></param>
    /// <param name="subDomain"></param>
    /// <returns></returns>
    public async Task RunAsync(GenerateOptions options, string subDomain) {
        this._logger.LogInformation("Starting code generation for AppId: {appId}", options.AppId);

        try {
            // 1. GenerateOptions → CodeEmitterOptions
            var emitterOptions = this._schemaConverter.ConvertToEmitterOptions(options);

            // // 2. Kintone スキーマ取得
            this._schemaProvider.SetDomain(subDomain);
            var schema = await this._schemaProvider.GetSchemaAsync(options.AppId, options.ApiToken);

            // 3. コード生成（ICodeEmitter）
            this._logger.LogInformation("Generating code...");
            var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage);
            var result = codeEmitter.Emit(schema, emitterOptions);

            // 4. 出力ディレクトリ作成
            Directory.CreateDirectory(options.OutputDirectory);

            // 5. ファイル出力
            this._fileWriter.WriteMainModel(result.MainModelCode, emitterOptions, options.AppId, schema.Revision);

            foreach (var sub in result.SubTableModels) {
                this._fileWriter.WriteSubTable(sub, emitterOptions, options.AppId, schema.Revision);
            }
            foreach (var helper in result.HelperClasses) {
                this._fileWriter.WriteHelper(helper, emitterOptions, options.AppId, schema.Revision);
            }

            this._logger.LogInformation("Code generation completed for AppId: {appId}", options.AppId);

        } catch (Exception ex) {
            this._logger.LogError(ex, "Error during code generation for AppId: {appId}", options.AppId);
            throw;
        }
    }
    /// <summary>
    /// Raw メタデータ出力の実行
    /// </summary>
    /// <param name="options"></param>
    /// <param name="subDomain"></param>
    /// <returns></returns>
    public async Task RunRawAsync(RawOptions options, string subDomain) {
        this._logger.LogInformation("Starting RAW metadata export for AppId: {appId}", options.AppId);

        try {
            // 1. ドメイン設定
            this._schemaProvider.SetDomain(subDomain);

            // 2. 生のメタデータ取得
            var metadata = await this._schemaProvider.GetMetadataAsync(options.AppId, options.ApiToken);

            // 3. JSON へシリアライズ
            var json = JsonSerializer.Serialize(
                metadata,
                new JsonSerializerOptions { WriteIndented = true, Encoder = options.EscapeUnicode ? null : System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }
            );

            // 4. 出力ディレクトリ作成
            Directory.CreateDirectory(options.OutputDirectory);

            // 5. 出力ファイルパス
            var filePath = Path.Combine(
                options.OutputDirectory,
                $"App{options.AppId}-{metadata.Revision}.metadata.json"
            );

            // 6. 上書きチェック
            if (File.Exists(filePath) && !options.OverwriteExistingFiles) {
                this._logger.LogWarning("File already exists and overwrite is disabled: {filePath}", filePath);
                return;
            }

            // 7. 書き込み
            await File.WriteAllTextAsync(filePath, json);

            this._logger.LogInformation("RAW metadata export completed: {filePath}", filePath);

        } catch (Exception ex) {
            this._logger.LogError(ex, "Error during RAW metadata export for AppId: {appId}", options.AppId);
            throw;
        }
    }
    /// <summary>
    /// メタデータ差分比較の実行
    /// </summary>
    /// <param name="options"></param>
    /// <param name="subDomain"></param>
    /// <returns></returns>
    public async Task RunDiffAsync(DiffOptions options, string subDomain) {
        this._logger.LogInformation("Starting metadata diff for AppId: {appId}", options.AppId);

        try {
            // 1. ドメイン設定
            this._schemaProvider.SetDomain(subDomain);

            // 2. 比較対象ファイルを決定
            string? previousFile = options.CompareWithFile;
            if (string.IsNullOrWhiteSpace(previousFile)) {
                previousFile = Directory
                    .EnumerateFiles(options.OutputDirectory, $"App{options.AppId}-*.metadata.json")
                    .OrderByDescending(f => f)
                    .FirstOrDefault();
            }
            if (previousFile == null || !File.Exists(previousFile)) {
                this._logger.LogError("No previous metadata file found for comparison.");
                return;
            }

            // 3. 比較対象のJSONを読み込む
            var previousJson = await File.ReadAllTextAsync(previousFile);
            var previous = JsonSerializer.Deserialize<KintoneAppMetadata>(previousJson);
            if (previous == null) {
                this._logger.LogError("Failed to deserialize previous metadata from file: {file}", previousFile);
                return;
            }

            // 4. 差分取得
            var diffs = await this._schemaProvider.CompareAsync(previous, options.AppId, options.ApiToken);
            if (diffs.Count == 0) {
                this._logger.LogInformation("No differences found between the metadata.");
                return;
            }

            // 5. 差分出力
            this._fileWriter.WriteDiffResults(diffs, options);

            this._logger.LogInformation("Metadata diff completed for AppId: {appId}", options.AppId);

        } catch (Exception ex) {
            this._logger.LogError(ex, "Error during metadata diff for AppId: {appId}", options.AppId);
            throw;
        }
    }
}
