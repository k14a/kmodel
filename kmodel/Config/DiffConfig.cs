using kmodel.Enums;
using kmodel.Options;

namespace kmodel.Config;

/// <summary>
/// config.json の Diff メタデータ出力エントリに対応する設定モデル
/// </summary>
public class DiffConfig {
    /// <summary>アプリID</summary>
    public int AppId { get; set; }

    /// <summary>APIトークン</summary>
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>出力ディレクトリ</summary>
    public string OutputDirectory { get; set; } = ".";

    /// <summary>既存ファイルを上書きするか</summary>
    public bool OverwriteExistingFiles { get; set; }

    /// <summary>比較対象のファイルパス</summary>
    public string? CompareWithFile { get; set; }

    /// <summary>出力フォーマット</summary>
    public DiffOutputFormats OutputFormat { get; set; } = DiffOutputFormats.Text;

    /// <summary>出力ファイルパス</summary>
    public string? DiffOutputFile { get; set; }

    /// <summary>比較対象ファイルが存在しない場合にエラーとするか</summary>
    public bool FailIfNoBaseFile { get; set; }

    /// <summary>Unicode エスケープを行うか</summary>
    public bool EscapeUnicode { get; set; } = true;

    /// <summary>JSON を整形して出力するか</summary>
    public bool Pretty { get; set; }

    /// <summary>
    /// DiffOptions へ変換する
    /// </summary>
    public DiffOptions ToOptions() => new() {
        AppId = this.AppId,
        ApiToken = this.ApiToken,
        OutputDirectory = this.OutputDirectory,
        OverwriteExistingFiles = this.OverwriteExistingFiles,
        CompareWithFile = this.CompareWithFile,
        OutputFormat = this.OutputFormat,
        DiffOutputFile = this.DiffOutputFile,
        FailIfNoBaseFile = this.FailIfNoBaseFile,
        EscapeUnicode = this.EscapeUnicode,
        Pretty = this.Pretty,
    };
}
