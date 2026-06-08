using kmodel.Options;

namespace kmodel.Config;

/// <summary>
/// config.json の Raw メタデータ出力エントリに対応する設定モデル
/// </summary>
public class RawConfig {
    /// <summary>アプリID</summary>
    public int AppId { get; set; }

    /// <summary>APIトークン</summary>
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>出力ディレクトリ</summary>
    public string? OutputDirectory { get; set; }

    /// <summary>Unicode エスケープを行うか</summary>
    public bool EscapeUnicode { get; set; } = true;

    /// <summary>既存ファイルを上書きするか</summary>
    public bool OverwriteExistingFiles { get; set; }

    /// <summary>JSON を整形して出力するか</summary>
    public bool Pretty { get; set; }

    /// <summary>
    /// RawOptions へ変換する
    /// </summary>
    public RawOptions ToOptions() => new() {
        AppId = this.AppId,
        ApiToken = this.ApiToken,
        OutputDirectory = this.OutputDirectory,
        EscapeUnicode = this.EscapeUnicode,
        OverwriteExistingFiles = this.OverwriteExistingFiles,
        Pretty = this.Pretty,
    };
}
