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
    /// コード生成設定リスト
    /// </summary>
    public List<GenerateConfig> Generates { get; set; } = [];
    /// <summary>
    /// Raw メタデータ出力設定リスト
    /// </summary>
    public List<RawConfig> Raws { get; set; } = [];
    /// <summary>
    /// Diff メタデータ出力設定リスト
    /// </summary>
    public List<DiffConfig> Diffs { get; set; } = [];
}
