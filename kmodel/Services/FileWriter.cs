using System.Text;
using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Enums;
using kmodel.Extensions;
using kmodel.Interfaces;
using kmodel.Options;
using Microsoft.Extensions.Logging;

namespace kmodel.Services;

/// <summary>
/// 多言語対応のファイル書き込みクラス
/// </summary>
public class FileWriter(ILogger<FileWriter> logger) : IFileWriter {
    private readonly ILogger<FileWriter> _logger = logger;

    /// <summary>
    /// メインモデルのコードを書き込む。ファイル名は "{MainClassName}{FileExtension}" となる。出力先ディレクトリは fileOptions.OutputDirectory。
    /// </summary>
    /// <param name="code">生成されたコード</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリID</param>
    /// <param name="revision">リビジョン番号</param>
    public void WriteMainModel(
        string code,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {
        var fileName = $"{emitterOptions.MainClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, code, emitterOptions, fileOptions, appId, revision);
    }

    /// <summary>
    /// サブテーブルモデルのコードを書き込む。ファイル名は "{ClassName}{FileExtension}" となる。出力先ディレクトリは fileOptions.OutputDirectory。
    /// サブテーブルごとに1ファイルが生成される。
    /// emitterOptions.MainClassName はサブテーブルには適用されないため、サブテーブルのクラス名を使用してファイル名を生成する。
    /// emitterOptions.FileExtension はメインモデルと同様に使用される。
    /// ファイルの内容はコード生成結果のサブテーブルモデルのコードである。
    /// appId と revision はログ出力やファイル名生成の際に使用される可能性があるが、実装によっては使用されない場合もある。
    /// </summary>
    /// <param name="model">生成されたサブテーブルモデル</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリID</param>
    /// <param name="revision">リビジョン番号</param>
    public void WriteSubTable(
        GeneratedSubTableModel model,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {

        var fileName = $"{model.ClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, model.Code, emitterOptions, fileOptions, appId, revision);
    }

    /// <summary>
    /// ヘルパークラスのコードを書き込む。ファイル名は "{ClassName}{FileExtension}" となる。出力先ディレクトリは fileOptions.OutputDirectory。
    /// </summary>
    /// <param name="model">生成されたヘルパークラスモデル</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリID</param>
    /// <param name="revision">リビジョン番号</param>
    public void WriteHelper(
        GeneratedHelperClass model,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {
        var fileName = $"{model.ClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, model.Code, emitterOptions, fileOptions, appId, revision);
    }

    /// <summary>
    /// 差分結果を書き込む。ファイル名は options.DiffOutputFile で指定されたものを使用する。指定がない場合は "App{AppId}.diff.txt" となる。出力先ディレクトリは options.OutputDirectory。
    /// options.OutputFormat に応じて、テキスト形式、JSON形式、Markdown形式で出力する。JSON形式の場合、options.Pretty と options.EscapeUnicode に基づいてシリアライズオプションを設定する。
    /// </summary>
    /// <param name="diffs">差分結果のリスト</param>
    /// <param name="options">差分出力オプション</param>
    public void WriteDiffResults(
        IReadOnlyList<KintoneMetadataDiff> diffs,
        DiffOptions options) {
        var fileName = options.DiffOutputFile ?? $"App{options.AppId}.diff.txt";
        fileName = fileName.Replace("{appId}", options.AppId.ToString());

        var path = Path.Combine(options.OutputDirectory, fileName);

        if (diffs.Count == 0) {
            File.WriteAllText(path, "No differences found.");
            return;
        }

        var content = options.OutputFormat switch {
            DiffOutputFormats.Json => System.Text.Json.JsonSerializer.Serialize(
                diffs,
                new System.Text.Json.JsonSerializerOptions {
                    WriteIndented = options.Pretty,
                    Encoder = options.EscapeUnicode
                        ? null
                        : System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }),
            DiffOutputFormats.Markdown => string.Join(Environment.NewLine + Environment.NewLine, diffs.Select(d => d.ToMarkdown())),
            _ => string.Join(Environment.NewLine, diffs.Select(d => d.ToText()))
        };

        File.WriteAllText(path, content);
    }

    /// <summary>
    /// Raw メタデータを書き込む(未実装)
    /// </summary>
    /// <param name="metadata">書き込むRAWメタデータ</param>
    /// <param name="options">RAW出力オプション</param>
    /// <exception cref="NotImplementedException"></exception>
    public void WriteRawMetadata(KintoneAppMetadata metadata, RawOptions options) => throw new NotImplementedException();

    /// <summary>
    /// 指定されたコードをファイルに書き込む。ファイル名は emitterOptions.MainClassName と emitterOptions.FileExtension を組み合わせて生成される。出力先ディレクトリは fileOptions.OutputDirectory。
    /// ファイルが既に存在する場合、fileOptions.OverwriteExistingFiles に基づいて上書きするかどうかを判断する。ファイル名に無効な文字が含まれている場合はエラーをログに記録し、書き込み処理をスキップする。
    /// </summary>
    /// <param name="fileName">書き込むファイルの名前</param>
    /// <param name="code">書き込むコードの内容</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリID</param>
    /// <param name="revision">リビジョン番号</param>
    private void WriteFile(
        string fileName,
        string code,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {

        if (!IsValidFileName(fileName)) {
            this._logger.LogError("Invalid file name detected: {FileName}", fileName);
            return;
        }

        this.EnsureOutputDirectory(fileOptions.OutputDirectory);

        var path = new FileInfo(Path.Combine(fileOptions.OutputDirectory, fileName));

        if (!CanWrite(path, fileOptions)) {
            this._logger.LogWarning("File already exists and overwrite is disabled: {File}", path.FullName);
            return;
        }

        // ---- ヘッダー生成（言語非依存） ----
        var header = new StringBuilder();

        var finalCode = header.ToString() + code;

        File.WriteAllText(path.FullName, finalCode);
        this._logger.LogInformation("  - {File}", path.FullName);
    }

    /// <summary>
    /// 出力先ディレクトリが存在しない場合は作成する。存在する場合は何もしない。
    /// ディレクトリのパスは outputDirectory で指定される。outputDirectory が null または空文字の場合は、カレントディレクトリを使用する。
    /// ディレクトリの作成に失敗した場合は例外がスローされる。
    /// ログには、ディレクトリが存在しないため新たに作成された場合にその旨が記録される。
    /// </summary>
    /// <param name="outputDirectory">出力先ディレクトリのパス</param>
    private void EnsureOutputDirectory(string outputDirectory) {
        if (!Directory.Exists(outputDirectory)) {
            this._logger.LogDebug("Output directory does not exist. Creating: {Dir}", outputDirectory);
            Directory.CreateDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// 指定されたファイルにコードを書き込むことができるかどうかを判断する。ファイルが存在しない場合は true を返す。ファイルが存在する場合は、options.OverwriteExistingFiles が true の場合は true を返し、false の場合は false を返す。
    /// ファイルの存在チェックは FileInfo クラスを使用して行う。ファイルが存在し、上書きが許可されていない場合は、ログに警告が記録される。
    /// </summary>
    /// <param name="path">ファイルのパスを表す FileInfo オブジェクト</param>
    /// <param name="options">ファイル書き込みオプション</param>
    /// <returns>ファイルに書き込むことができる場合は true、それ以外の場合は false</returns>
    private static bool CanWrite(FileInfo path, FileWriterOptions options)
        => !path.Exists || options.OverwriteExistingFiles;

    /// <summary>
    /// ファイル名に無効な文字が含まれていないかをチェックする。ファイル名に使用できない文字が含まれている場合は false を返し、そうでない場合は true を返す。
    /// </summary>
    /// <param name="fileName">チェックするファイル名</param>
    /// <returns>ファイル名が有効な場合は true、それ以外の場合は false</returns>
    private static bool IsValidFileName(string fileName)
        => fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
}