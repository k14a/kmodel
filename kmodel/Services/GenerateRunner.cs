using kmodel.Options;
using kmodel.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using kmodel.Logging;

namespace kmodel.Services;

public class GenerateRunner : RunnerBase<GenerateRunner> {
    private readonly IFileWriter _fileWriter;
    private readonly ISchemaConverter _schemaConverter;
    private readonly ISchemaProvider _schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory;

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

    public Task RunAsync(GenerateOptions options, string subDomain)
        => this.ExecuteAsync(
            $"Starting code generation for AppId: {options.AppId}",
            async () => {
                var emitterOptions = this._schemaConverter.ConvertToEmitterOptions(options);

                this._schemaProvider.SetDomain(subDomain);
                var schema = await this._schemaProvider.GetSchemaAsync(options.AppId, options.ApiToken);

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
