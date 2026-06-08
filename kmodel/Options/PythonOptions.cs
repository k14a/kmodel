namespace kmodel.Options;

/// <summary>
/// Python 固有のコード生成オプション
/// </summary>
public class PythonOptions {
    /// <summary>
    /// 生成するモジュール名
    /// </summary>
    public string ModuleName { get; set; } = "kintone_models";

    /// <summary>
    /// type hint を付与するか
    /// </summary>
    public bool UseTypeHint { get; set; } = true;
}
