using System.CommandLine;
using kmodel.Enums;

namespace kmodel.Commands;

/// <summary>
/// OptionFactory クラスは、kmodel CLI の各コマンドで使用される共通のオプションを定義するためのファクトリークラスです。サブドメイン、アプリID、APIトークン、出力ディレクトリ、言語、Unicodeエスケープの有無、整形オプション、Diff 出力フォーマット、比較用ファイルパスなどのオプションを提供します。これらのオプションは、GenerateCommandBuilder、DiffCommandBuilder、RawCommandBuilder などのコマンドビルダークラスで利用され、ユーザーがコマンドを実行する際に必要な情報を入力できるようにします。
/// </summary>
public static class OptionFactory {
    /// <summary>
    /// サブドメインを指定するオプションを作成する静的メソッド。ユーザーが Kintone のサブドメインを入力できるようにするためのオプションで、例えば "example" を入力すると、Kintone の URL が "https://example.cybozu.com" になることを想定しています。このオプションは必須であり、ユーザーがコマンドを実行する際にサブドメインを指定しないとエラーになります。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string> SubDomain() =>
        new("--sub-domain") {
            Required = false,
            Description = "Kintone subdomain (required unless --fields is used)"
        };

    /// <summary>
    /// アプリIDを指定するオプションを作成する静的メソッド。ユーザーが Kintone アプリの ID を入力できるようにするためのオプションで、例えば "123" を入力すると、Kintone のアプリ ID が 123 であることを想定しています。このオプションは必須であり、ユーザーがコマンドを実行する際にアプリ ID を指定しないとエラーになります。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<int> AppId() =>
        new("--app-id") {
            Required = true,
            Description = "Kintone App ID"
        };

    /// <summary>
    /// APIトークンを指定するオプションを作成する静的メソッド。ユーザーが Kintone API トークンを入力できるようにするためのオプションで、例えば "ABCDEF" を入力すると、その文字列が API トークンとして使用されます。このオプションは必須であり、ユーザーがコマンドを実行する際に API トークン を指定しないとエラーになります。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string> ApiToken() =>
        new("--api-token") {
            Required = false,
            Description = "Kintone API Token (required unless --fields is used)"
        };

    /// <summary>
    /// 出力ディレクトリを指定するオプションを作成する静的メソッド。ユーザーが出力ファイルを保存するディレクトリのパスを入力できるようにするためのオプションで、例えば "./output" を入力すると、出力ファイルが "output" ディレクトリに保存されることを想定しています。このオプションは必須ではなく、ユーザーが指定しない場合はデフォルトでカレントディレクトリが使用されます。
    /// </summary>
    /// <param name="description">オプションの説明</param>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string?> OutputDirectory(string description) =>
        new("--out-dir") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = description
        };

    /// <summary>
    /// コード生成の言語を指定するオプションを作成する静的メソッド。ユーザーがコード生成の対象となるプログラミング言語を入力できるようにするためのオプションで、例えば "CSharp" を入力すると、C# のコードが生成されることを想定しています。このオプションは必須ではなく、ユーザーが指定しない場合はデフォルトで "CSharp" が使用されます。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string> Language() =>
        new("--lang") {
            Required = false,
            DefaultValueFactory = _ => "CSharp",
            Description = "Programming language for code generation"
        };

    /// <summary>
    /// Unicode エスケープの有無を指定するオプションを作成する静的メソッド。ユーザーが出力に含まれるフィールドコードをエスケープするかどうかを指定できるようにするためのオプションで、例えば "true" を入力すると、フィールドコードがエスケープされます。このオプションは必須ではなく、ユーザーが指定しない場合はデフォルトで "true" が使用されます。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<bool> EscapeUnicode() =>
        new("--escape") {
            Required = false,
            DefaultValueFactory = _ => true,
            Description = "Escape field codes in output"
        };

    /// <summary>
    /// 出力を整形するかどうかを指定するオプションを作成する静的メソッド。ユーザーが出力 JSON を整形して表示するかどうかを指定できるようにするためのオプションで、例えば "true" を入力すると、出力 JSON が整形されます。このオプションは必須ではなく、ユーザーが指定しない場合はデフォルトで "false" が使用されます。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<bool> PrettyPrint() =>
        new("--pretty") {
            Required = false,
            DefaultValueFactory = _ => false,
            Description = "Pretty print the output JSON"
        };

    /// <summary>
    /// 差分出力形式を指定するオプションを作成する静的メソッド。ユーザーが差分出力の形式を指定できるようにするためのオプションで、例えば "Text" を入力すると、差分出力がテキスト形式で表示されます。このオプションは必須ではなく、ユーザーが指定しない場合はデフォルトで "Text" が使用されます。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<DiffOutputFormats> DiffOutputFormat() =>
        new("--format") {
            Required = false,
            DefaultValueFactory = _ => DiffOutputFormats.Text,
            Description = "Output format for diff (Text or Json or Markdown)"
        };

    /// <summary>
    /// 比較対象のファイルを指定するオプションを作成する静的メソッド。ユーザーが以前のメタデータ JSON ファイルを指定できるようにするためのオプションで、例えば "./previous.json" を入力すると、そのファイルと現在のメタデータを比較します。このオプションは必須ではなく、ユーザーが指定しない場合は比較が行われません。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string?> CompareWithFile() =>
        new("--compare-with") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = "Path to previous metadata JSON file for comparison (optional)"
        };

    /// <summary>
    /// フィールド定義ファイルを指定するオプションを作成する静的メソッド。ユーザーが kbackup からエクスポートされたフィールド定義の JSON ファイルを指定できるようにするためのオプションで、例えば "./fields.json" を入力すると、そのファイルからフィールド定義を読み込みます。このオプションは必須ではなく、ユーザーが指定しない場合はフィールド定義が読み込まれません。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string?> FieldsFile() =>
        new("--fields") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = "Path to fields JSON file from kbackup (optional)"
        };

    /// <summary>
    /// 名前テーブルテンプレートのエクスポート先を指定するオプションを作成する静的メソッド。ユーザーが名前テーブルテンプレートの出力先ファイルパスを指定できるようにするためのオプションで、例えば "./name_table_template.json" を入力すると、そのファイルに名前テーブルテンプレートがエクスポートされます。このオプションは必須ではなく、ユーザーが指定しない場合は名前テーブルテンプレートはエクスポートされません。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string?> ExportNameTableTemplate() =>
        new("--export-name-table-template") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = "Export name table template (JSON) and exit. No model code will be generated."
        };

    /// <summary>
    /// 名前テーブルのエクスポート先を指定するオプションを作成する静的メソッド。ユーザーが名前テーブルの出力先ファイルパスを指定できるようにするためのオプションで、例えば "./name_table.json" を入力すると、そのファイルに名前テーブルがエクスポートされます。このオプションは必須ではなく、ユーザーが指定しない場合は名前テーブルはエクスポートされません。
    /// </summary>
    /// <returns>構築された Option インスタンス</returns>
    public static Option<string?> NameTable() =>
        new("--name-table") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = "Path to name table JSON file for property name conversion (used when regenerating models)"
        };
}
