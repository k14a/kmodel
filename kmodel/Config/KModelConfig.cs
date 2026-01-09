using kmodel.Options;

namespace kmodel.Config;

/// <summary>
/// KModel 設定
/// </summary>
public class KModelConfig {
    /// <summary>
    /// サブドメイン
    /// </summary>
    public string SubDomain { get; set; } = string.Empty;
    /// <summary>
    /// コード生成オプションリスト
    /// </summary>
    public List<GenerateOptions> Generates { get; set; } = [];
    /// <summary>
    /// Raw メタデータ出力オプションリスト
    /// </summary>
    public List<RawOptions> Raws { get; set; } = [];
    /// <summary>
    /// Diff メタデータ出力オプションリスト
    /// </summary>
    public List<DiffOptions> Diffs { get; set; } = [];
}