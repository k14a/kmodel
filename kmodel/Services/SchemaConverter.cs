using Microsoft.Extensions.Logging;
using KintoneNetLibrary.CodeGen.Domain.Options;
using kmodel.Options;
using kmodel.Interfaces;

namespace kmodel.Services;

/// <summary>
/// スキーマ変換クラス
/// </summary>
/// <param name="logger"></param>
public class SchemaConverter(ILogger<SchemaConverter> logger) : ISchemaConverter {
    private readonly ILogger<SchemaConverter> _logger = logger;

    /// <summary>
    /// GenerateOptions を CodeEmitterOptions に変換する
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public CodeEmitterOptions ConvertToEmitterOptions(GenerateOptions options) {
        this._logger.LogDebug("Converting GenerateOptions to CodeEmitterOptions for AppId: {appId}", options.AppId);

        var emitterOptions = new CodeEmitterOptions {
            Namespace = options.Namespace,
            MainClassName = options.ClassName ?? "KintoneModel",
            UseRecord = options.UseRecord,
            UseKintoneNetLibrary = !options.PureMode,
            GenerateEnums = options.GenerateEnums,
            OutputDirectory = options.OutputDirectory,
            OverwriteExistingFiles = options.OverwriteExistingFiles
        };

        emitterOptions.NormalizePaths();
        emitterOptions.EnsureDefaults(options.AppId);

        this._logger.LogDebug(
            "Conversion completed: Namespace={Namespace}, Class={ClassName}, Output={OutputDirectory}",
            emitterOptions.Namespace,
            emitterOptions.MainClassName,
            emitterOptions.OutputDirectory
        );

        return emitterOptions;
    }
}