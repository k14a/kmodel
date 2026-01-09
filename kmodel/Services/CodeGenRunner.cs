using Microsoft.Extensions.Logging;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using kmodel.Config;

namespace kmodel.Services;

public class CodeGenRunner(
    IFileWriter fileWriter,
    ISchemaConverter schemaConverter,
    ISchemaProvider schemaProvider,
    ICodeEmitter codeEmitter,
    ILogger<CodeGenRunner> logger) {
    private readonly IFileWriter _fileWriter = fileWriter;
    private readonly ISchemaConverter _schemaConverter = schemaConverter;
    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly ICodeEmitter _codeEmitter = codeEmitter;
    private readonly ILogger<CodeGenRunner> _logger = logger;

    public async Task RunAsync(GenerateOptions options, string subDomain) {
        this._logger.LogInformation("Starting code generation for AppId: {appId}", options.AppId);

        try {
            // 1. GenerateOptions → CodeEmitterOptions
            var emitterOptions = this._schemaConverter.ConvertToEmitterOptions(options);

            // // 2. Kintone スキーマ取得
            // this._logger.LogInformation("Fetching schema from Kintone...");
            // var config = this._configLoader.Load("./kmodel.config.json");
            this._schemaProvider.SetDomain(subDomain);
            var schema = await this._schemaProvider.GetSchemaAsync(options.AppId, options.ApiToken);

            // 3. コード生成（ICodeEmitter）
            this._logger.LogInformation("Generating code...");
            var result = this._codeEmitter.Emit(schema, emitterOptions);

            // 4. 出力ディレクトリ作成
            Directory.CreateDirectory(emitterOptions.OutputDirectory);

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
    public async Task RunRawAsync(RawOptions options, string subDomain) {
        throw new NotImplementedException();
    }
}
