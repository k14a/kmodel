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

    // ================================
    // メインモデル
    // ================================
    public void WriteMainModel(
        string code,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {
        var fileName = $"{emitterOptions.MainClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, code, emitterOptions, fileOptions, appId, revision);
    }

    // ================================
    // サブテーブル
    // ================================
    public void WriteSubTable(
        GeneratedSubTableModel model,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {
        var fileName = $"{model.ClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, model.Code, emitterOptions, fileOptions, appId, revision);
    }

    // ================================
    // ヘルパークラス
    // ================================
    public void WriteHelper(
        GeneratedHelperClass model,
        CodeEmitterOptions emitterOptions,
        FileWriterOptions fileOptions,
        int appId,
        int revision) {
        var fileName = $"{model.ClassName}{emitterOptions.FileExtension}";
        this.WriteFile(fileName, model.Code, emitterOptions, fileOptions, appId, revision);
    }

    // ================================
    // 差分出力（言語非依存）
    // ================================
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
    /// <param name="metadata"></param>
    /// <param name="options"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void WriteRawMetadata(KintoneAppMetadata metadata, RawOptions options) => throw new NotImplementedException();

    // ================================
    // 共通ファイル出力処理
    // ================================
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

    // ================================
    // ユーティリティ
    // ================================
    private void EnsureOutputDirectory(string outputDirectory) {
        if (!Directory.Exists(outputDirectory)) {
            this._logger.LogDebug("Output directory does not exist. Creating: {Dir}", outputDirectory);
            Directory.CreateDirectory(outputDirectory);
        }
    }

    private static bool CanWrite(FileInfo path, FileWriterOptions options)
        => !path.Exists || options.OverwriteExistingFiles;

    private static bool IsValidFileName(string fileName)
        => fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
}