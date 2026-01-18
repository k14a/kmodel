using kmodel.Enums;

namespace kmodel.Options;

/// <summary>
/// Diff メタデータ出力オプション
/// </summary>
public class DiffOptions {
    /// <summary>
    /// アプリID
    /// </summary>
    public int AppId { get; set; }
    /// <summary>
    /// APIトークン
    /// </summary>
    public string ApiToken { get; set; } = string.Empty;
    /// <summary>
    /// 出力ディレクトリ
    /// </summary>
    public string OutputDirectory { get; set; } = ".";
    /// <summary>
    /// 既存ファイルを上書きするか
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }
    /// <summary>
    /// 比較対象のファイルパス
    /// </summary>
    public string? CompareWithFile { get; set; }
    /// <summary>
    /// 出力フォーマット
    /// </summary>
    public DiffOutputFormats OutputFormat { get; set; } = DiffOutputFormats.Text;
    /// <summary>
    /// 出力ファイルパス
    /// </summary>
    public string? DiffOutputFile { get; set; }
    /// <summary>
    /// 比較対象ファイルが存在しない場合にエラーとするか
    /// </summary>
    public bool FailIfNoBaseFile { get; set; } = false;
    /// <summary>
    /// Unicode エスケープを行うか
    /// </summary>
    public bool EscapeUnicode { get; set; } = true;
    public bool Pretty { get; set; } = false;
}