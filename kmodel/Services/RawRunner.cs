using kmodel.Options;
using System.Text.Json;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace kmodel.Services;

public class RawRunner(
    ISchemaProvider schemaProvider,
    ILogger<RawRunner> logger) : RunnerBase<RawRunner>(logger) {
    private readonly ISchemaProvider _schemaProvider = schemaProvider;

    public Task RunAsync(RawOptions options, string subDomain)
        => this.ExecuteAsync(
            $"Starting RAW metadata export for AppId: {options.AppId}",
            async () => {
                this._schemaProvider.SetDomain(subDomain);

                var metadata = await this._schemaProvider.GetMetadataAsync(options.AppId, options.ApiToken);

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
