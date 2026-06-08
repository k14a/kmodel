using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Options;

namespace kmodel.Config;

/// <summary>
/// config.json のコード生成エントリに対応する設定モデル
/// </summary>
public class GenerateConfig {
    /// <summary>アプリID</summary>
    public int AppId { get; set; }

    /// <summary>APIトークン</summary>
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>列挙型を生成するか</summary>
    public bool GenerateEnums { get; set; }

    /// <summary>生成言語</summary>
    public GenerateLanguages GenerateLanguage { get; set; } = GenerateLanguages.CSharp;

    /// <summary>サブテーブルクラスを別ファイルに分割するか</summary>
    public bool SplitSubTableFiles { get; set; } = true;

    /// <summary>補助クラスを別ファイルに分割するか</summary>
    public bool SplitHelperFiles { get; set; } = true;

    /// <summary>生成ファイルの先頭に出力するコメント</summary>
    public string? HeaderComment { get; set; }

    /// <summary>生成するメインクラス名（C#・Python 共通）</summary>
    public string? ClassName { get; set; }

    /// <summary>C# 固有オプション</summary>
    public CSharpOptions CSharp { get; set; } = new();

    /// <summary>Python 固有オプション</summary>
    public PythonOptions Python { get; set; } = new();

    /// <summary>ファイル出力オプション</summary>
    public FileWriterOptions FileWriter { get; set; } = new();

    /// <summary>フィールド定義ファイルのパス</summary>
    public string FieldsFile { get; set; } = string.Empty;

    /// <summary>名前テーブルテンプレートのエクスポート先ファイルパス</summary>
    public string ExportNameTableTemplate { get; set; } = string.Empty;

    /// <summary>名前テーブルファイルのパス</summary>
    public string NameTable { get; set; } = string.Empty;

    /// <summary>
    /// GenerateOptions へ変換する
    /// </summary>
    public GenerateOptions ToOptions() => new() {
        AppId = this.AppId,
        ApiToken = this.ApiToken,
        GenerateEnums = this.GenerateEnums,
        GenerateLanguage = this.GenerateLanguage,
        SplitSubTableFiles = this.SplitSubTableFiles,
        SplitHelperFiles = this.SplitHelperFiles,
        HeaderComment = this.HeaderComment,
        ClassName = this.ClassName,
        CSharp = this.CSharp,
        Python = this.Python,
        FileWriter = this.FileWriter,
        FieldsFile = this.FieldsFile,
        ExportNameTableTemplate = this.ExportNameTableTemplate,
        NameTable = this.NameTable,
    };
}
