using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Enums;

namespace kmodel.Options;

/// <summary>
/// コード生成オプション
/// </summary>
public class GenerateOptions {
    #region <<for CodeEmitterOptions>>
    /// <summary>
    /// アプリID
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// APIトークン
    /// </summary>
    public string ApiToken { get; set; } = string.Empty;

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
    /// サブテーブルクラスを別ファイルに分割するか
    /// </summary>
    public bool SplitSubTableFiles { get; set; } = true;

    /// <summary>
    /// pure モードの補助クラス（UserInfo など）を別ファイルに分割するか
    /// </summary>
    public bool SplitHelperFiles { get; set; } = true;

    /// <summary>
    /// 生成ファイルの先頭に出力するコメント
    /// </summary>
    public string? HeaderComment { get; set; }
    #endregion // <<for CodeEmitterOptions>>

    #region <<for CSharpEmitterOptions>>
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
    /// partial class を生成するか
    /// </summary>
    public bool UsePartial { get; set; } = true;

    /// <summary>
    /// #nullable enable を付与するか
    /// </summary>
    public bool NullableEnabled { get; set; } = true;
    #endregion // <<for CSharpEmitterOptions>>

    #region <<for PythonEmitterOptions>>
    /// <summary>
    /// 生成するモジュール名
    /// </summary>
    public string ModuleName { get; set; } = "kintone_models";

    /// <summary>
    /// type hint を付与するか
    /// </summary>
    public bool UseTypeHint { get; set; } = true;
    #endregion // <<for PythonEmitterOptions>>

    #region <<for FileWriterOptions>>
    /// <summary>
    /// 出力ディレクトリ
    /// </summary>
    public string OutputDirectory { get; set; } = ".";

    /// <summary>
    /// 既存ファイルを上書きするか
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }
    #endregion // <<for FileWriterOptions>>

    /// <summary>
    /// フィールド定義ファイルのパス。これが指定された場合、Kintone API からスキーマを取得せず、代わりにこのファイルからフィールド定義を読み取ります。ファイルは JSON 形式で、Kintone API のフィールド定義と同じ構造を持つ必要があります。このオプションは、API アクセスが制限されている環境や、既存のフィールド定義を再利用したい場合に便利です。
    /// </summary>
    public string FieldsFile { get; set; } = string.Empty;

    /// <summary>
    /// エクスポート名テーブルのテンプレートファイルのパス。これが指定された場合、コード生成時にフィールドコードとフィールド名の対応表をこのテンプレートファイルに基づいて生成します。ファイルは JSON 形式で、フィールドコードをキー、フィールド名を値とするオブジェクトの構造を持つ必要があります。このオプションは、生成されるコードでフィールドコードを人間が理解しやすい名前にマッピングしたい場合に便利です。
    /// </summary>
    public string ExportNameTableTemplate { get; set; } = string.Empty;

    /// <summary>
    /// エクスポート名テーブルの出力ファイルのパス。これが指定された場合、コード生成時にフィールドコードとフィールド名の対応表をこのファイルに出力します。ファイルは JSON 形式で、フィールドコードをキー、フィールド名を値とするオブジェクトの構造になります。このオプションは、生成されるコードでフィールドコードを人間が理解しやすい名前にマッピングしたい場合に便利です。
    /// </summary>
    public string NameTable { get; set; } = string.Empty;
}