using kmodel.Options;
using System.Text.Json;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Services;

/// <summary>
/// KintoneアプリのメタデータをRAW形式で出力する Runner。Kintone API から取得したメタデータを JSON 形式で出力します。出力先は標準出力またはファイルのいずれかを選択できます。
/// options.OutputDirectory が null、空文字、または "-" の場合は標準出力に出力されます。それ以外の場合は、指定されたディレクトリに "App{AppId}-{Revision}.metadata.json" というファイル名で出力されます。既存のファイルが存在する場合、options.OverwriteExistingFiles に基づいて上書きするかどうかを判断します。
/// options.Pretty が true の場合は、出力される JSON はインデントされて読みやすくなります。options.EscapeUnicode が true の場合は、Unicode 文字がエスケープされます。
/// 例外が発生した場合は、エラーメッセージがログに記録されます。正常に出力が完了した場合は、出力先の情報がログに記録されます。
/// なお、この Runner は Kintone アプリのメタデータをそのまま出力するため、コード生成は行いません。コード生成が必要な場合は GenerateRunner を使用してください。
/// </summary>
/// <param name="schemaProvider">スキーマ提供サービス</param>
/// <param name="logger">ロガー</param>
public class RawRunner(
    ISchemaProvider schemaProvider,
    ILogger<RawRunner> logger) : RunnerBase<RawRunner>(logger) {
    private readonly ISchemaProvider _schemaProvider = schemaProvider;

    public Task RunAsync(RawOptions options, string subDomain)
        => this.ExecuteAsync(
            $"Starting RAW metadata export for AppId: {options.AppId}",
            async () => {
                this._schemaProvider.SetDomain(subDomain);

                var access = new ApiTokenAccess(subDomain, options.ApiToken);
                var metadata = await this._schemaProvider.GetMetadataAsync(access.Domain, options.ApiToken, options.AppId);

                var json = JsonSerializer.Serialize(
                    metadata,
                    new JsonSerializerOptions {
                        WriteIndented = options.Pretty,
                        Encoder = options.EscapeUnicode
                            ? null
                            : System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });

                // -----------------------------------------
                // ① OutputDirectory が null/空/"-" の場合 → 標準出力
                // -----------------------------------------
                if (string.IsNullOrWhiteSpace(options.OutputDirectory) || options.OutputDirectory == "-") {
                    Console.WriteLine(json);
                    this.Info("RAW metadata output to STDOUT");
                    return;
                }

                // -----------------------------------------
                // ② 通常のファイル出力
                // -----------------------------------------
                Directory.CreateDirectory(options.OutputDirectory);

                var filePath = Path.Combine(
                    options.OutputDirectory,
                    $"App{options.AppId}-{metadata.Revision}.metadata.json");

                if (File.Exists(filePath) && !options.OverwriteExistingFiles) {
                    this.Info($"File already exists and overwrite is disabled: {filePath}");
                    return;
                }

                await File.WriteAllTextAsync(filePath, json);

                this.Info($"RAW metadata export completed: {filePath}");
            });
}
