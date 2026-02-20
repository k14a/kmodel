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
    ILoggerFactory loggerFactory) : RunnerBase<GenerateRunner>(loggerFactory.CreateLogger<GenerateRunner>()) {

    private readonly IFileWriter _fileWriter = fileWriter;
    private readonly ISchemaConverter _schemaConverter = schemaConverter;
    private readonly IFieldParser _fieldParser = parser;
    private readonly IMetadataConverter _converter = converter;
    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory = codeEmitterFactory;
    private readonly INameTableGenerator _nameTableGenerator = nameTableGenerator;
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
                var emitterOptions = this._schemaConverter.ConvertToEmitterOptions(options);

                // Providerの切り替え
                ISchemaProvider provider;
                if (!string.IsNullOrEmpty(options.FieldsFile)) {
                    // JSONモード
                    StaticLogMessages.MethodInfo(this._logger, message: $"Using LocalJsonSchemaProvider with fields file: {options.FieldsFile}");
                    provider = new LocalJsonSchemaProvider(
                        options.FieldsFile,
                        this._fieldParser,
                        this._converter,
                        this._loggerFactory.CreateLogger<LocalJsonSchemaProvider>());

                } else {
                    StaticLogMessages.MethodInfo(this._logger, message: $"Using SchemaProvider with sub-domain: {subDomain}");
                    provider = this._schemaProvider;
                    provider.SetDomain(subDomain);
                }

                // スキーマ取得
                KintoneAppSchema schema;

                if (!string.IsNullOrEmpty(options.FieldsFile)) {
                    // JSONモード
                    schema = await provider.GetSchemaAsync(string.Empty, string.Empty, options.AppId);

                } else {
                    // APIモード
                    var access = new ApiTokenAccess(subDomain, options.ApiToken);
                    schema = await provider.GetSchemaAsync(access.Domain, options.ApiToken, options.AppId);
                }

                if (!string.IsNullOrEmpty(options.ExportNameTableTemplate)) {
                    StaticLogMessages.MethodInfo(this._logger, message: $"Generating name table template: {options.ExportNameTableTemplate}");

                    var nameTable = this._nameTableGenerator.Generate(schema);
                    var json = JsonSerializer.Serialize(nameTable, DefaultJsonOptions.Default);
                    File.WriteAllText(options.ExportNameTableTemplate, json);

                    StaticLogMessages.MethodInfo(this._logger, message: $"Name table template generated: {options.ExportNameTableTemplate}");

                    // 名前テーブルの生成が完了したらコード生成は行わずに終了する
                    return;
                }

                // コード生成
                StaticLogMessages.MethodStart(this._logger, message: "Generating code...");
                var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage);
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
            });
}
