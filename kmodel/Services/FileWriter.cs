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
    /// メインモデルのコードを書き込む。出力先ディレクトリは fileOptions.OutputDirectory。
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    public void WriteMainModel(string fileName, string code, FileWriterOptions fileOptions) {
        this.WriteFile(fileName, code, fileOptions);
    }

    /// <summary>
    /// サブテーブルモデルのコードを書き込む。出力先ディレクトリは fileOptions.OutputDirectory。
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    public void WriteSubTable(string fileName, string code, FileWriterOptions fileOptions) {
        this.WriteFile(fileName, code, fileOptions);
    }

    /// <summary>
    /// ヘルパークラスのコードを書き込む。出力先ディレクトリは fileOptions.OutputDirectory。
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    public void WriteHelper(string fileName, string code, FileWriterOptions fileOptions) {
        this.WriteFile(fileName, code, fileOptions);
    }

    /// <summary>
    /// 差分結果を書き込む。ファイル名は options.DiffOutputFile で指定されたものを使用する。指定がない場合は "App{AppId}.diff.txt" となる。
    /// </summary>
    /// <param name="content">シリアライズ済みの差分内容</param>
    /// <param name="options">差分出力オプション</param>
    public void WriteDiffResults(string content, DiffOptions options) {
        var fileName = options.DiffOutputFile ?? $"App{options.AppId}.diff.txt";
        fileName = fileName.Replace("{appId}", options.AppId.ToString());
        var path = Path.Combine(options.OutputDirectory, fileName);
        File.WriteAllText(path, content);
    }

    /// <summary>
    /// 指定されたコードをファイルに書き込む。ファイルが既に存在する場合、fileOptions.OverwriteExistingFiles に基づいて上書きするかどうかを判断する。
    /// </summary>
    private void WriteFile(
        string fileName,
        string code,
        FileWriterOptions fileOptions) {

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

        File.WriteAllText(path.FullName, code);
        this._logger.LogInformation("  - {File}", path.FullName);
    }

    /// <summary>
    /// 出力先ディレクトリが存在しない場合は作成する。
    /// </summary>
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
