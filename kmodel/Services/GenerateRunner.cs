using kmodel.Options;
using kmodel.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using kmodel.Logging;
using KintoneNetLibrary.Domain.Access;
using KintoneNetLibrary.CodeGen.Infrastructure.Services;
using KintoneNetLibrary.Domain.Interfaces;
using KintoneNetLibrary.CodeGen.Domain.Schemas;
using System.Text.Json;
using KintoneNetLibrary.Domain.Common;
using KintoneNetLibrary.CodeGen.Domain.Models;

namespace kmodel.Services;

/// <summary>
/// Kintoneアプリのメタデータからコードを生成するRunner。
/// 指定された言語でコードを生成し、ファイルに出力する責務を持ちます。
/// </summary>
public class GenerateRunner(
    IFileWriter fileWriter,
    ISchemaConverter schemaConverter,
    IFieldParser parser,
    IMetadataConverter converter,
    ISchemaProvider schemaProvider,
    ICodeEmitterFactory codeEmitterFactory,
    INameTableGenerator nameTableGenerator,
    INameConverterFactory nameConverterFactory,
    ILoggerFactory loggerFactory) : RunnerBase<GenerateRunner>(loggerFactory.CreateLogger<GenerateRunner>()) {

    private readonly IFileWriter _fileWriter = fileWriter;
    private readonly ISchemaConverter _schemaConverter = schemaConverter;
    private readonly IFieldParser _fieldParser = parser;
    private readonly IMetadataConverter _converter = converter;
    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory = codeEmitterFactory;
    private readonly INameTableGenerator _nameTableGenerator = nameTableGenerator;
    private readonly INameConverterFactory _nameConverterFactory = nameConverterFactory;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    /// <summary>
    /// コード生成処理を実行する。options で指定された AppId とサブドメインを使用して Kintone API からスキーマを取得し、指定された言語でコードを生成してファイルに出力する。
    /// </summary>
    /// <param name="options">コード生成オプション</param>
    /// <param name="subDomain">Kintone サブドメイン</param>
    /// <returns>非同期タスク</returns>
    public Task RunAsync(GenerateOptions options, string subDomain)
        => this.ExecuteAsync(
            $"Starting code generation for AppId: {options.AppId}",
            async () => {
                var provider = this.CreateProvider(options, subDomain);
                var schema = await this.LoadSchemaAsync(provider, options, subDomain);

                // 1. export-name-table-template
                if (!string.IsNullOrEmpty(options.ExportNameTableTemplate)) {
                    await this.ExportNameTableTemplateAsync(schema, options);
                    return;
                }

                // 2. name-table の読み込み
                var nameTable = this.LoadNameTableIfExists(options);

                // 3. コード生成
                await this.GenerateCodeAsync(schema, options, nameTable);
            });

    /// <summary>
    /// ISchemaProvider を生成する。options.FieldsFile が指定されている場合は LocalJsonSchemaProvider を使用し、そうでない場合は SchemaProvider を使用する。
    /// </summary>
    /// <param name="options">コード生成オプション</param>
    /// <param name="subDomain">Kintone サブドメイン</param>
    /// <returns>ISchemaProvider インスタンス</returns>
    private ISchemaProvider CreateProvider(GenerateOptions options, string subDomain) {
        if (!string.IsNullOrEmpty(options.FieldsFile)) {
            StaticLogMessages.MethodInfo(this._logger, message: $"Using LocalJsonSchemaProvider with fields file: {options.FieldsFile}");
            return new LocalJsonSchemaProvider(
                options.FieldsFile,
                this._fieldParser,
                this._converter,
                this._loggerFactory.CreateLogger<LocalJsonSchemaProvider>());
        }

        StaticLogMessages.MethodInfo(this._logger, message: $"Using SchemaProvider with sub-domain: {subDomain}");
        this._schemaProvider.SetDomain(subDomain);
        return this._schemaProvider;
    }

    /// <summary>
    /// Kintone API からスキーマを取得する。options.FieldsFile が指定されている場合はローカルの JSON ファイルからスキーマを読み込む。そうでない場合は API トークンを使用して Kintone API からスキーマを取得する。
    /// </summary>
    /// <param name="provider">スキーマプロバイダー</param>
    /// <param name="options">コード生成オプション</param>
    /// <param name="subDomain">Kintone サブドメイン</param>
    /// <returns>KintoneAppSchema インスタンス</returns>
    private async Task<KintoneAppSchema> LoadSchemaAsync(ISchemaProvider provider, GenerateOptions options, string subDomain) {
        if (!string.IsNullOrEmpty(options.FieldsFile)) {
            return await provider.GetSchemaAsync(string.Empty, string.Empty, options.AppId);
        }

        var access = new ApiTokenAccess(subDomain, options.ApiToken);
        return await provider.GetSchemaAsync(access.Domain, options.ApiToken, options.AppId);
    }

    /// <summary>
    /// スキーマから名前テーブルのテンプレートを生成して JSON ファイルに出力する。options.ExportNameTableTemplate で出力先のファイルパスを指定する。
    /// </summary>
    /// <param name="schema">KintoneAppSchema インスタンス</param>
    /// <param name="options">コード生成オプション</param>
    /// <returns>非同期タスク</returns>
    private async Task ExportNameTableTemplateAsync(KintoneAppSchema schema, GenerateOptions options) {
        StaticLogMessages.MethodInfo(this._logger, message: $"Generating name table template: {options.ExportNameTableTemplate}");

        var nameTable = this._nameTableGenerator.Generate(schema);
        var json = JsonSerializer.Serialize(nameTable, DefaultJsonOptions.Default);

        await File.WriteAllTextAsync(options.ExportNameTableTemplate, json);

        StaticLogMessages.MethodInfo(this._logger, message: $"Name table template generated: {options.ExportNameTableTemplate}");
    }

    /// <summary>
    /// options.NameTable で指定された JSON ファイルから名前テーブルを読み込む。options.NameTable が null または空文字列の場合は null を返す。
    /// </summary>
    /// <param name="options">コード生成オプション</param>
    /// <returns>NameTable インスタンスまたは null</returns>
    private NameTable? LoadNameTableIfExists(GenerateOptions options) {
        if (string.IsNullOrEmpty(options.NameTable)) {
            return null;
        }

        StaticLogMessages.MethodInfo(this._logger, message: $"Loading name table: {options.NameTable}");

        var json = File.ReadAllText(options.NameTable);
        var table = JsonSerializer.Deserialize<NameTable>(json, DefaultJsonOptions.Default);

        StaticLogMessages.MethodInfo(this._logger, message: $"Name table loaded: {options.NameTable}");

        return table;
    }

    /// <summary>
    /// KintoneAppSchema と GenerateOptions を使用してコードを生成し、ファイルに出力する。options.NameTable で指定された名前テーブルが存在する場合はそれを使用してコードを生成する。
    /// </summary>
    /// <param name="schema">KintoneAppSchema インスタンス</param>
    /// <param name="options">コード生成オプション</param>
    /// <param name="nameTable">NameTable インスタンスまたは null</param>
    /// <returns>非同期タスク</returns>
    private async Task GenerateCodeAsync(KintoneAppSchema schema, GenerateOptions options, NameTable? nameTable) {
        StaticLogMessages.MethodStart(this._logger, message: "Generating code...");

        var emitterOptions = this._schemaConverter.ConvertToEmitterOptions(options);

        // NameConverterFactory に nameTable を渡す
        var nameConverter = this._nameConverterFactory.Create(options.GenerateLanguage, nameTable);

        var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage, nameConverter);
        var result = codeEmitter.Emit(schema, emitterOptions);

        Directory.CreateDirectory(options.OutputDirectory);

        var writerOptions = options.ToFileWriterOptions();
        this._fileWriter.WriteMainModel(result.MainModelCode, emitterOptions, writerOptions, options.AppId, schema.Revision);

        foreach (var sub in result.SubTableModels) {
            this._fileWriter.WriteSubTable(sub, emitterOptions, writerOptions, options.AppId, schema.Revision);
        }

        foreach (var helper in result.HelperClasses) {
            this._fileWriter.WriteHelper(helper, emitterOptions, writerOptions, options.AppId, schema.Revision);
        }

        StaticLogMessages.MethodFinish(this._logger, message: $"Code generation completed for AppId: {options.AppId}");
    }
}
