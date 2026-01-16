using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Enums;

namespace kmodel.Options;

/// <summary>
/// コード生成オプション
/// </summary>
public class GenerateOptions {
    /// <summary>
    /// アプリID
    /// </summary>
    public int AppId { get; set; }
    /// <summary>
    /// APIトークン
    /// </summary>
    public string ApiToken { get; set; } = string.Empty;
    /// <summary>
    /// 名前空間
    /// </summary>
    public string Namespace { get; set; } = "KintoneModels";
    /// <summary>
    /// クラス名
    /// </summary>
    public string? ClassName { get; set; }
    /// <summary>
    /// クラスの代わりにレコードを使用するか
    /// </summary>
    public bool UseRecord { get; set; }
    /// <summary>
    /// Pure モードを使用するか
    /// </summary>
    /// <notes>falseの場合、KintoneNetLibrary 依存のコードが生成されます。</notes>
    public bool PureMode { get; set; }
    /// <summary>
    /// 列挙型を生成するか
    /// </summary>
    public bool GenerateEnums { get; set; }
    /// <summary>
    /// 生成言語
    /// </summary>
    public GenerateLanguages GenerateLanguage { get; set; } = GenerateLanguages.CSharp;
    /// <summary>
    /// 出力ディレクトリ
    /// </summary>
    public string OutputDirectory { get; set; } = ".";
    /// <summary>
    /// 既存ファイルを上書きするか
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }
}