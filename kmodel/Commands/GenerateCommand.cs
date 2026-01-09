using KintoneNetLibrary.Application.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.CodeGen.Domain.Schemas;
using kmodel.Interfaces;
using kmodel.Options;
using kmodel.Services;

namespace kmodel.Commands;

/// <summary>
/// モデル生成コマンド
/// </summary>
/// <param name="metadataApi"></param>
/// <param name="schemaConverter"></param>
/// <param name="codeEmitter"></param>
/// <param name="fileWriter"></param>
public class GenerateCommand(
    IKintoneAppMetadataApi metadataApi,
    ISchemaConverter schemaConverter,
    ICodeEmitter codeEmitter,
    IFileWriter fileWriter) {

    private readonly IKintoneAppMetadataApi _metadataApi = metadataApi;
    private readonly ISchemaConverter _schemaConverter = schemaConverter;
    private readonly ICodeEmitter _codeEmitter = codeEmitter;
    private readonly IFileWriter _fileWriter = fileWriter;

    /// <summary>
    /// コマンド実行
    /// </summary>
    /// <param name="opts"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(GenerateOptions opts) {
        throw new NotImplementedException();
        // Console.WriteLine($"Fetching fields.json for App {opts.AppId} ...");

        // // 1. Kintone から fields.json を取得
        // var metadata = await _metadataApi.GetAppMetadataAsync(opts.AppId);

        // // 2. Domain モデルへ変換
        // var schema = _schemaConverter.Convert(metadata);

        // // 3. CodeEmitterOptions を構築
        // var emitterOptions = BuildEmitterOptions(opts, schema);

        // // 4. CodeGen を実行
        // Console.WriteLine("Generating C# models...");
        // var result = _codeEmitter.Emit(schema, emitterOptions);

        // // 5. ファイル出力
        // Console.WriteLine("Writing files...");
        // WriteFiles(result, emitterOptions);

        // Console.WriteLine("✔ Model generation completed.");
    }

    // private static CodeEmitterOptions BuildEmitterOptions(GenerateOptions opts, KintoneAppSchema schema) {
    //     return new CodeEmitterOptions {
    //         Namespace = opts.Namespace,
    //         MainClassName = opts.ClassName ?? $"App{schema.AppId}",
    //         UseRecord = opts.UseRecord,
    //         UsePartial = true,
    //         NullableEnabled = true,
    //         UseKintoneNetLibrary = !opts.PureMode,
    //         GenerateEnums = opts.GenerateEnums,
    //         SplitSubtableFiles = true,
    //         SplitHelperFiles = true
    //     };
    // }

    // private void WriteFiles(GeneratedModelResult result, CodeEmitterOptions options) {
    //     // メインモデル
    //     this._fileWriter.WriteMainModel(result.MainModelCode, options, options.AppId, result.Revision);

    //     // サブテーブル
    //     foreach (var sub in result.SubtableModels){
    //         this._fileWriter.WriteSubtable(sub, options, options.AppId, result.Revision);
    //     }
    //     // pure モードの補助クラス
    //     foreach (var helper in result.HelperClasses){
    //         this._fileWriter.WriteHelper(helper, options, options.AppId, result.Revision);
    //     }
    // }
}