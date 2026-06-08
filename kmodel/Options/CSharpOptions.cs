namespace kmodel.Options;

/// <summary>
/// C# 固有のコード生成オプション
/// </summary>
public class CSharpOptions {
    /// <summary>
    /// 名前空間
    /// </summary>
    public string Namespace { get; set; } = "KintoneModels";

    /// <summary>
    /// クラスの代わりにレコードを使用するか
    /// </summary>
    public bool UseRecord { get; set; }

    /// <summary>
    /// partial class を生成するか
    /// </summary>
    public bool UsePartial { get; set; } = true;

    /// <summary>
    /// #nullable enable を付与するか
    /// </summary>
    public bool NullableEnabled { get; set; } = true;

    /// <summary>
    /// KintoneNetLibrary に依存しないコードを生成するか（Pure モード）
    /// </summary>
    public bool PureMode { get; set; }
}
