using kmodel.Options;
using kmodel.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using kmodel.Logging;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Services;

/// <summary>
/// Kintoneアプリのメタデータからコードを生成するRunner。
/// 指定された言語でコードを生成し、ファイルに出力する責務を持ちます。
/// </summary>
public class GenerateRunner : RunnerBase<GenerateRunner> {
    private readonly IFileWriter _fileWriter;
    private readonly ISchemaConverter _schemaConverter;
    private readonly ISchemaProvider _schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory;

    /// <summary>
    /// GenerateRunner のコンストラクタ。必要なサービスを依存性注入で受け取る。
    /// </summary>
    /// <param name="fileWriter">ファイル書き込みサービス</param>
    /// <param name="schemaConverter">スキーマ変換サービス</param>
    /// <param name="schemaProvider">スキーマ提供サービス</param>
    /// <param name="codeEmitterFactory">コード生成ファクトリ</param>
    /// <param name="logger">ロガー</param>
    public GenerateRunner(
        IFileWriter fileWriter,
        ISchemaConverter schemaConverter,
        ISchemaProvider schemaProvider,
        ICodeEmitterFactory codeEmitterFactory,
        ILogger<GenerateRunner> logger) : base(logger) {

        this._fileWriter = fileWriter;
        this._schemaConverter = schemaConverter;
        this._schemaProvider = schemaProvider;
        this._codeEmitterFactory = codeEmitterFactory;
    }

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

                this._schemaProvider.SetDomain(subDomain);
                var access = new ApiTokenAccess(subDomain, options.ApiToken);
                var schema = await this._schemaProvider.GetSchemaAsync(access.Domain, options.ApiToken, options.AppId);

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
