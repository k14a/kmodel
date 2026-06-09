using kmodel.Options;
using kmodel.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using Microsoft.Extensions.Logging;
using kmodel.Logging;
using KintoneNetLibrary.CodeGen.Domain.Schemas;
using System.Text.Json;
using KintoneNetLibrary.Domain.Common;
using KintoneNetLibrary.CodeGen.Domain.Models;
using System.Text.Encodings.Web;
using System.Text;
using KintoneNetLibrary.Domain.Enums;

namespace kmodel.Services;

/// <summary>
/// Kintoneアプリのメタデータからコードを生成するRunner。
/// 指定された言語でコードを生成し、ファイルに出力する責務を持ちます。
/// </summary>
public class GenerateRunner(
    IFileWriter fileWriter,
    IEmitterOptionsConverter schemaConverter,
    ISchemaProvider schemaProvider,
    ICodeEmitterFactory codeEmitterFactory,
    INameTableGenerator nameTableGenerator,
    INameConverterFactory nameConverterFactory,
    ILocalSchemaProviderFactory localSchemaProviderFactory,
    ILogger<GenerateRunner> logger) : RunnerBase<GenerateRunner>(logger) {

    private readonly IFileWriter _fileWriter = fileWriter;
    private readonly IEmitterOptionsConverter _schemaConverter = schemaConverter;
    private readonly ISchemaProvider _schemaProvider = schemaProvider;
    private readonly ICodeEmitterFactory _codeEmitterFactory = codeEmitterFactory;
    private readonly INameTableGenerator _nameTableGenerator = nameTableGenerator;
    private readonly INameConverterFactory _nameConverterFactory = nameConverterFactory;
    private readonly ILocalSchemaProviderFactory _localSchemaProviderFactory = localSchemaProviderFactory;

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
            return this._localSchemaProviderFactory.Create(options.FieldsFile);
        }

        StaticLogMessages.MethodInfo(this._logger, message: $"Using SchemaProvider with sub-domain: {subDomain}");
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

        return await provider.GetSchemaAsync(subDomain, options.ApiToken, options.AppId);
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
        var nameConverter = this._nameConverterFactory.Create(options.GenerateLanguage, null);
        foreach (var (fieldCode, mapping) in nameTable) {
            mapping.Property ??= nameConverter.ToPropertyName(mapping.Label, fieldCode, true);
        }

        var ext = Path.GetExtension(options.ExportNameTableTemplate).ToLowerInvariant();

        switch (ext) {
            case ".json":
                await this.ExportAsJsonAsync(nameTable, options.ExportNameTableTemplate);
                break;
            case ".csv":
                this.ExportAsDelimited(nameTable, options.ExportNameTableTemplate, ',');
                break;
            case ".tsv":
                this.ExportAsDelimited(nameTable, options.ExportNameTableTemplate, '\t');
                break;
            default:
                StaticLogMessages.MethodInfo(this._logger, message: $"Unsupported export format: {ext}. Only .json is supported.");
                return;
        }

        StaticLogMessages.MethodInfo(this._logger, message: $"Name table template generated: {options.ExportNameTableTemplate}");
    }

    /// <summary>
    /// 名前テーブルを JSON ファイルに出力する。path で出力先のファイルパスを指定する。
    /// </summary>
    /// <param name="table">名前テーブル</param>
    /// <param name="path">出力先のファイルパス</param>
    /// <returns>非同期タスク</returns>
    private async Task ExportAsJsonAsync(NameTable table, string path) {
        var jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        var json = JsonSerializer.Serialize(table, jsonOptions);

        await File.WriteAllTextAsync(path, json);

    }

    /// <summary>
    /// 名前テーブルを区切り文字形式（CSV/TSV）でファイルに出力する。path で出力先のファイルパスを指定し、delimiter で区切り文字を指定する。delimiter は ','（カンマ）または '\t'（タブ）を想定している。
    /// </summary>
    /// <param name="table">名前テーブル</param>
    /// <param name="path">出力先のファイルパス</param>
    /// <param name="delimiter">区切り文字</param>
    private void ExportAsDelimited(NameTable table, string path, char delimiter) {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine($"FieldCode{delimiter}Label{delimiter}FieldType{delimiter}SubTable{delimiter}Property");
        foreach (var (fieldCode, mapping) in table) {
            writer.WriteLine(
                $"{Escape(fieldCode, delimiter)}{delimiter}" +
                $"{Escape(mapping.Label, delimiter)}{delimiter}" +
                $"{mapping.FieldType}{delimiter}" +
                $"{mapping.SubTable ?? string.Empty}{delimiter}" +
                $"{mapping.Property ?? string.Empty}"
            );
        }
    }

    /// <summary>
    /// CSV/TSV 形式の値をエスケープする。値に区切り文字、改行、またはダブルクォートが含まれている場合は、値全体をダブルクォートで囲み、ダブルクォート自体は2つのダブルクォートに置換する。そうでない場合は値をそのまま返す。
    /// </summary>
    /// <param name="value">エスケープする値</param>
    /// <param name="delimiter">区切り文字</param>
    /// <returns>エスケープされた値</returns>
    internal static string Escape(string? value, char delimiter) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");

        if (escaped.Contains(delimiter) ||
            escaped.Contains('\r') ||
            escaped.Contains('\n') ||
            escaped.Contains('"')) {
            return $"\"{escaped}\"";
        }

        return escaped;
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

        var path = options.NameTable;
        var ext = Path.GetExtension(path).ToLowerInvariant();

        StaticLogMessages.MethodInfo(this._logger, null, $"Loading name table: {path}");

        NameTable table = ext switch {
            ".json" => this.LoadNameTableFromJson(path),
            ".csv" => this.LoadNameTableFromDelimited(path, ','),
            ".tsv" => this.LoadNameTableFromDelimited(path, '\t'),
            _ => throw new InvalidOperationException($"Unsupported name-table extension: {ext}")
        };

        StaticLogMessages.MethodInfo(this._logger, null, $"Name table loaded: {path}");

        return table;
    }

    /// <summary>
    /// JSON ファイルから名前テーブルを読み込む。path で読み込むファイルのパスを指定する。ファイルが存在しない場合や JSON の形式が正しくない場合は例外が発生する。
    /// </summary>
    /// <param name="path">読み込む JSON ファイルのパス</param>
    /// <returns>NameTable インスタンス</returns>
    private NameTable LoadNameTableFromJson(string path) {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<NameTable>(json, DefaultJsonOptions.Default) ?? [];
    }

    /// <summary>
    /// 区切り文字形式（CSV/TSV）ファイルから名前テーブルを読み込む。path で読み込むファイルのパスを指定し、delimiter で区切り文字を指定する。delimiter は ','（カンマ）または '\t'（タブ）を想定している。ファイルが存在しない場合や形式が正しくない場合は例外が発生する。
    /// </summary>
    /// <param name="path">読み込むファイルのパス</param>
    /// <param name="delimiter">区切り文字</param>
    /// <returns>NameTable インスタンス</returns>
    private NameTable LoadNameTableFromDelimited(string path, char delimiter) {
        var table = new NameTable();

        using var reader = new StreamReader(path, new UTF8Encoding(false));

        // 1行目はヘッダー
        var header = reader.ReadLine();
        if (header == null) {
            return table;
        }

        string? line;
        while ((line = reader.ReadLine()) != null) {
            var cols = ParseDelimitedLine(line, delimiter);
            if (cols.Length < 5) { continue; }

            var fieldCode = cols[0];
            var label = cols[1];
            var type = cols[2];
            var subTable = string.IsNullOrWhiteSpace(cols[3]) ? null : cols[3];
            var property = cols[4];

            var fieldType = Enum.TryParse<KintoneFieldType>(type, out var ft) ? ft : KintoneFieldType.Unknown;
            table.Set(fieldCode, new FieldNameMapping {
                Label = label,
                FieldType = fieldType,
                SubTable = subTable,
                Property = string.IsNullOrWhiteSpace(property) ? null : property
            });
        }

        return table;
    }

    /// <summary>
    /// スキーマから名前テーブルのテンプレートを生成して JSON ファイルに出力する。options.ExportNameTableTemplate で出力先のファイルパスを指定する。
    /// 生成される名前テーブルのテンプレートは、フィールドコード、フィールドのラベル、フィールドタイプ、サブテーブルの有無を含む構造になっており、コード生成時にプロパティ名を指定するためのテンプレートとして使用されることを想定しています。 
    /// </summary>
    /// <param name="line">解析する行の文字列</param>
    /// <param name="delimiter">区切り文字</param>
    /// <returns>解析された列の配列</returns>
    internal static string[] ParseDelimitedLine(string line, char delimiter) {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++) {
            var c = line[i];

            if (c == '"') {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"') {
                    sb.Append('"');
                    i++;
                } else {
                    inQuotes = !inQuotes;
                }
            } else if (c == delimiter && !inQuotes) {
                result.Add(sb.ToString());
                sb.Clear();
            } else {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result.ToArray();
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

        var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage);
        var result = codeEmitter.Emit(schema, emitterOptions, nameConverter);

        Directory.CreateDirectory(options.FileWriter.OutputDirectory);

        var writerOptions = options.FileWriter;
        this._fileWriter.WriteMainModel(
            $"{emitterOptions.MainClassName}{emitterOptions.FileExtension}",
            result.MainModelCode,
            writerOptions);

        foreach (var sub in result.SubTableModels) {
            this._fileWriter.WriteSubTable(
                $"{sub.ClassName}{emitterOptions.FileExtension}",
                sub.Code,
                writerOptions);
        }

        foreach (var helper in result.HelperClasses) {
            this._fileWriter.WriteHelper(
                $"{helper.ClassName}{emitterOptions.FileExtension}",
                helper.Code,
                writerOptions);
        }

        StaticLogMessages.MethodFinish(this._logger, message: $"Code generation completed for AppId: {options.AppId}");
    }
}
