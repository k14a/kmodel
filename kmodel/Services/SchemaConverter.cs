using KintoneNetLibrary.CodeGen.Domain.Enums;
using KintoneNetLibrary.CodeGen.Domain.Options;
using kmodel.Options;
using kmodel.Interfaces;
using Microsoft.Extensions.Logging;

namespace kmodel.Services;

/// <summary>
/// GenerateOptions を CodeEmitterOptions に変換するコンバータ
/// </summary>
public class SchemaConverter(ILogger<SchemaConverter> logger) : ISchemaConverter {
    private readonly ILogger<SchemaConverter> _logger = logger;

    /// <summary>
    /// GenerateOptions を言語ごとの CodeEmitterOptions に変換する
    /// </summary>
    public CodeEmitterOptions ConvertToEmitterOptions(GenerateOptions options) {
        this._logger.LogDebug(
            "Converting GenerateOptions to CodeEmitterOptions for AppId: {appId}, Language: {lang}",
            options.AppId,
            options.GenerateLanguage
        );

        CodeEmitterOptions emitterOptions = options.GenerateLanguage switch {
            GenerateLanguages.CSharp => ConvertToCSharpOptions(options),
            GenerateLanguages.Python => ConvertToPythonOptions(options),
            _ => throw new NotSupportedException(
                $"GenerateLanguage '{options.GenerateLanguage}' is not supported.")
        };

        emitterOptions.EnsureDefaults(options.AppId);

        this._logger.LogDebug(
            "Conversion completed: MainClass={MainClassName}, GenerateEnums={GenerateEnums}, SplitSubTableFiles={SplitSubTableFiles}, SplitHelperFiles={SplitHelperFiles}",
            emitterOptions.MainClassName,
            emitterOptions.GenerateEnums,
            emitterOptions.SplitSubTableFiles,
            emitterOptions.SplitHelperFiles
        );

        return emitterOptions;
    }

    /// <summary>
    /// C# 用の CodeEmitterOptions へ変換
    /// </summary>
    private static CSharpEmitterOptions ConvertToCSharpOptions(GenerateOptions options) {
        var cs = new CSharpEmitterOptions {
            // 共通
            MainClassName = options.ClassName,
            GenerateEnums = options.GenerateEnums,
            SplitSubTableFiles = options.SplitSubTableFiles,
            SplitHelperFiles = options.SplitHelperFiles,
            HeaderComment = options.HeaderComment,

            // C# 固有
            Namespace = options.Namespace ?? "KintoneModels",
            UseRecord = options.UseRecord,
            UsePartial = options.UsePartial,
            NullableEnabled = options.NullableEnabled,
            UseKintoneNetLibrary = !options.PureMode
        };

        return cs;
    }

    /// <summary>
    /// Python 用の CodeEmitterOptions へ変換
    /// </summary>
    private static PythonEmitterOptions ConvertToPythonOptions(GenerateOptions options) {
        var py = new PythonEmitterOptions {
            // 共通
            MainClassName = options.ClassName,
            GenerateEnums = options.GenerateEnums,
            SplitSubTableFiles = options.SplitSubTableFiles,
            SplitHelperFiles = options.SplitHelperFiles,
            HeaderComment = options.HeaderComment,

            // Python 固有
            ModuleName = options.ModuleName ?? "kintone_models",
            UseTypeHint = options.UseTypeHint
        };

        return py;
    }
}