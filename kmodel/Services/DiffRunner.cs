using kmodel.Options;
using kmodel.Interfaces;
using KintoneNetLibrary.Domain.Entities;
using System.Text.Json;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Services;

/// <summary>
/// Kintoneアプリのメタデータの差分を出力する Runner。前回出力したメタデータと現在のメタデータを比較し、変更点をレポートする。
/// </summary>
/// <param name="schemaProvider">スキーマプロバイダー</param>
/// <param name="fileWriter">ファイルライター</param>
/// <param name="logger">ロガー</param>
public class DiffRunner(
    ISchemaProvider schemaProvider,
    IFileWriter fileWriter,
    ILogger<DiffRunner> logger) : RunnerBase<DiffRunner>(logger) {

    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly IFileWriter _fileWriter = fileWriter;

    /// <summary>
    /// Config.Diffs に基づいて差分出力処理を実行する。前回出力したメタデータファイルと現在のメタデータを比較し、変更点をファイルに出力する。
    /// </summary>
    /// <param name="options">差分オプション</param>
    /// <param name="subDomain">サブドメイン</param>
    /// <returns>実行タスク</returns>
    public Task RunAsync(DiffOptions options, string subDomain)
        => this.ExecuteAsync(
            $"Starting metadata diff for AppId: {options.AppId}",
            async () => {
                this._schemaProvider.SetDomain(subDomain);

                string? previousFile = options.CompareWithFile;
                if (string.IsNullOrWhiteSpace(previousFile)) {
                    previousFile = Directory
                        .EnumerateFiles(options.OutputDirectory, $"App{options.AppId}-*.metadata.json")
                        .OrderByDescending(f => f)
                        .FirstOrDefault();
                }

                if (previousFile == null || !File.Exists(previousFile)) {
                    this.Info("No previous metadata file found for comparison.");
                    return;
                }

                var previousJson = await File.ReadAllTextAsync(previousFile);
                var previous = JsonSerializer.Deserialize<KintoneAppMetadata>(previousJson);

                if (previous == null) {
                    this.Info($"Failed to deserialize previous metadata from file: {previousFile}");
                    return;
                }

                var access = new ApiTokenAccess(subDomain, options.ApiToken);
                var diffs = await this._schemaProvider.CompareAsync(previous, access.Domain, options.ApiToken, options.AppId);

                if (diffs.Count == 0) {
                    this.Info("No differences found between the metadata.");
                    return;
                }

                this._fileWriter.WriteDiffResults(diffs, options);

                this.Info($"Metadata diff completed for AppId: {options.AppId}");
            });
}
