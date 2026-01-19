namespace kmodel.Options;

/// <summary>
/// Raw メタデータ出力オプション
/// </summary>
public class RawOptions {
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
    public string? OutputDirectory { get; set; }
    /// <summary>
    /// Unicode エスケープを行うか
    /// </summary>
    public bool EscapeUnicode { get; set; } = true;
    /// <summary>
    /// 既存ファイルを上書きするか
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }
    /// <summary>
    /// JSONを整形して出力するか
    /// </summary>
    public bool Pretty { get; set; } = false;
}