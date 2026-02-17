using KintoneNetLibrary.CodeGen.Domain.Enums;
using KintoneNetLibrary.CodeGen.Domain.Options;
using kmodel.Options;
using kmodel.Interfaces;
using Microsoft.Extensions.Logging;

namespace kmodel.Services;

/// <summary>
/// GenerateOptions を言語ごとの CodeEmitterOptions に変換するクラス。GenerateRunner で使用され、ユーザーが指定したオプションをコード生成のためのオプションに変換します。
/// 現在は C# と Python に対応していますが、将来的には他の言語にも対応できるように設計されています。変換の際には、GenerateOptions の共通オプションを CodeEmitterOptions の共通プロパティにマッピングし、言語固有のオプションも適切にマッピングします。
/// 変換後の CodeEmitterOptions は、コード生成の際に使用されます。例えば、C# の場合は CSharpEmitterOptions、Python の場合は PythonEmitterOptions になります。変換の過程で、必要に応じてデフォルト値を設定したり、オプションの整合性をチェックしたりすることもできます。
/// </summary>
/// <param name="logger">ロガー</param>
public class SchemaConverter(ILogger<SchemaConverter> logger) : ISchemaConverter {
    private readonly ILogger<SchemaConverter> _logger = logger;

    /// <summary>
    /// GenerateOptions を CodeEmitterOptions に変換する。options.GenerateLanguage に基づいて、適切な CodeEmitterOptions のサブクラスに変換します。サポートされていない言語が指定された場合は NotSupportedException をスローします。
    /// 変換の際には、共通のオプションを CodeEmitterOptions の共通プロパティにマッピングし、言語固有のオプションも適切にマッピングします。変換後の CodeEmitterOptions は、コード生成の際に使用されます。
    /// 例えば、C# の場合は CSharpEmitterOptions、Python の場合は PythonEmitterOptions になります。変換の過程で、必要に応じてデフォルト値を設定したり、オプションの整合性をチェックしたりすることもできます。
    /// </summary>
    /// <param name="options">変換元の GenerateOptions</param>
    /// <returns>変換後の CodeEmitterOptions</returns>
    /// <exception cref="NotSupportedException">サポートされていない言語が指定された場合にスローされます</exception>
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
    /// C# 用の CodeEmitterOptions へ変換する。GenerateOptions のオプションを CSharpEmitterOptions のプロパティにマッピングします。共通のオプションは CodeEmitterOptions の共通プロパティにマッピングされ、C# 固有のオプションも適切にマッピングされます。
    /// </summary>
    /// <param name="options">変換元の GenerateOptions</param>
    /// <returns>変換後の CSharpEmitterOptions</returns>
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
    /// Python 用の CodeEmitterOptions へ変換する。GenerateOptions のオプションを PythonEmitterOptions のプロパティにマッピングします。共通のオプションは CodeEmitterOptions の共通プロパティにマッピングされ、Python 固有のオプションも適切にマッピングされます。
    /// </summary>
    /// <param name="options">変換元の GenerateOptions</param>
    /// <returns>変換後の PythonEmitterOptions</returns>
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