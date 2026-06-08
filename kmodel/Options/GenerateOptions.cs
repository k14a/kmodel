using KintoneNetLibrary.CodeGen.Domain.Enums;

namespace kmodel.Options;

/// <summary>
/// コード生成オプション
/// </summary>
public class GenerateOptions {
    #region <<共通 / CodeEmitterOptions>>
    /// <summary>
    /// アプリID
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// APIトークン
    /// </summary>
    public string ApiToken { get; set; } = string.Empty;

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
    /// 補助クラス（UserInfo など）を別ファイルに分割するか
    /// </summary>
    public bool SplitHelperFiles { get; set; } = true;

    /// <summary>
    /// 生成ファイルの先頭に出力するコメント
    /// </summary>
    public string? HeaderComment { get; set; }

    /// <summary>
    /// 生成するメインクラス名（C#・Python 共通）
    /// </summary>
    public string? ClassName { get; set; }
    #endregion // <<共通 / CodeEmitterOptions>>

    #region <<言語固有オプション>>
    /// <summary>
    /// C# 固有オプション
    /// </summary>
    public CSharpOptions CSharp { get; set; } = new();

    /// <summary>
    /// Python 固有オプション
    /// </summary>
    public PythonOptions Python { get; set; } = new();
    #endregion // <<言語固有オプション>>

    #region <<ファイル出力>>
    /// <summary>
    /// ファイル出力オプション
    /// </summary>
    public FileWriterOptions FileWriter { get; set; } = new();
    #endregion // <<ファイル出力>>

    /// <summary>
    /// フィールド定義ファイルのパス。指定された場合、Kintone API の代わりにこのファイルからフィールド定義を読み取る。
    /// </summary>
    public string FieldsFile { get; set; } = string.Empty;

    /// <summary>
    /// 名前テーブルテンプレートのエクスポート先ファイルパス。指定された場合、コード生成は行わずテンプレートを出力する。
    /// </summary>
    public string ExportNameTableTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 名前テーブルファイルのパス。フィールドコードとプロパティ名の対応表を読み込む。
    /// </summary>
    public string NameTable { get; set; } = string.Empty;
}
