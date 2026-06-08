using kmodel.Options;

namespace kmodel.Interfaces;

/// <summary>
/// ファイル書き込みインターフェース
/// </summary>
public interface IFileWriter {
    /// <summary>
    /// メインモデルファイルを書き込む
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    void WriteMainModel(string fileName, string code, FileWriterOptions fileOptions);
    /// <summary>
    /// サブテーブルモデルファイルを書き込む
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    void WriteSubTable(string fileName, string code, FileWriterOptions fileOptions);
    /// <summary>
    /// ヘルパークラスファイルを書き込む
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="code">生成されたコード</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    void WriteHelper(string fileName, string code, FileWriterOptions fileOptions);
    /// <summary>
    /// 差分結果を書き込む
    /// </summary>
    /// <param name="content">シリアライズ済みの差分内容</param>
    /// <param name="options">差分出力オプション（出力先パス決定に使用）</param>
    void WriteDiffResults(string content, DiffOptions options);
}
