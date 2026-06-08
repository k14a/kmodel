using System.Text;
using KintoneNetLibrary.Domain.Entities;
using KintoneNetLibrary.Domain.Enums;

namespace kmodel.Extensions;

/// <summary>
/// DiffResult 拡張メソッド
/// </summary>
public static class KintoneMetadataDiffExtensions {
    /// <summary>
    /// DiffResult をテキスト形式で出力する。フィールドの変更内容をわかりやすく表示するための形式で、Before と After のフィールドタイプとラベルを表示し、変更されたプロパティがある場合はそれもリストアップします。サブテーブルの差分も同様にテキスト形式で表示されます。
    /// </summary>
    /// <param name="diff">DiffResult インスタンス</param>
    /// <returns>テキスト形式の差分結果</returns>
    public static string ToText(this KintoneMetadataDiff diff) {
        var sb = new StringBuilder();

        sb.AppendLine($"Diff Summary (Rev {diff.BeforeRevision} → Rev {diff.AfterRevision})");
        sb.AppendLine($"[{diff.DiffType}] {diff.GetFieldName()}");

        if (diff.Before != null) {
            sb.AppendLine($"  Before: {diff.Before.FieldType} / {diff.Before.GetLabelWithCode()}");
        }

        if (diff.After != null) {
            sb.AppendLine($"  After : {diff.After.FieldType} / {diff.After.GetLabelWithCode()}");
        }

        if (diff.ChangedProperties.Count > 0) {
            sb.AppendLine("  Changed Properties:");
            foreach (var p in diff.ChangedProperties) {
                sb.AppendLine($"    - {p}");
            }
        }

        if (diff.SubTableDiffs.Count > 0) {
            sb.AppendLine("  SubTable Diffs:");
            foreach (var sub in diff.SubTableDiffs) {
                var subText = sub.ToText()
                    .Replace("\n", "\n    "); // インデント調整
                sb.AppendLine($"    {subText}");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// KintoneFieldMetadata のラベルとフィールドコードを組み合わせた文字列を取得する拡張メソッド。フィールドのラベルとコードをわかりやすく表示するための形式で、例えば "Customer Name (customer_name)" のような形式で返されます。フィールドラベルが存在しない場合は "(no label)" と表示されます。
    /// 例えば、field.GetLabelWithCode() と呼び出すと、その KintoneFieldMetadata インスタンスのフィールドラベルとフィールドコードを組み合わせた文字列が返されます。フィールドラベルが "Customer Name" でフィールドコードが "customer_name" の場合、"Customer Name (customer_name)" という文字列が返されます。フィールドラベルが null の場合は "(no label) (customer_name)" という形式になります。
    /// </summary>
    /// <param name="field">KintoneFieldMetadata インスタンス</param>
    /// <returns>ラベルとコードを組み合わせた文字列</returns>
    private static string GetLabelWithCode(this KintoneFieldMetadata field) {
        var label = field.FieldLabel ?? "(no label)";
        return $"{label} ({field.FieldCode})";
    }

    /// <summary>
    /// DiffResult を Markdown 形式で出力する。DiffResult の内容を Markdown 形式のテキストに変換します。Markdown 形式は、見出しやリストなどの構造を持つテキスト形式で、GitHub やその他のプラットフォームで広く使用されています。これにより、差分結果をわかりやすく表示することができます。
    /// 例えば、diff.ToMarkdown() と呼び出すと、その DiffResult インスタンスの内容が Markdown 形式の文字列として返されます。Markdown 文字列には、DiffType、BeforeRevision、AfterRevision、Before フィールドの情報、After フィールドの情報、変更されたプロパティのリスト、およびサブテーブルの差分が含まれます。見出しやリストを使用して、差分結果をわかりやすく整理しています。
    /// </summary>
    /// <param name="diff">DiffResult インスタンス</param>
    /// <returns>Markdown 形式の差分結果</returns>
    public static string ToMarkdown(this KintoneMetadataDiff diff) {
        var sb = new StringBuilder();

        // タイトル行
        sb.AppendLine($"## Diff Summary (Rev {diff.BeforeRevision} → {diff.AfterRevision})");
        sb.AppendLine($"### {diff.DiffType}: `{diff.GetFieldName()}`");
        sb.AppendLine();

        // DiffTypeごとの出力
        switch (diff.DiffType) {
            case KintoneMetadataDiffTypes.Added:
                AppendAfter(sb, diff);
                break;

            case KintoneMetadataDiffTypes.Removed:
                AppendBefore(sb, diff);
                break;

            case KintoneMetadataDiffTypes.Changed:
            case KintoneMetadataDiffTypes.ChangedOptions:
                AppendBefore(sb, diff);
                AppendAfter(sb, diff);
                AppendChangedProperties(sb, diff);
                break;
        }

        // サブテーブル差分
        if (diff.SubTableDiffs.Count > 0) {
            sb.AppendLine("- **SubTable Diffs:**");
            foreach (var sub in diff.SubTableDiffs) {
                var md = Indent(sub.ToMarkdown(), 2);
                sb.AppendLine(md);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// DiffResult の Before フィールドの情報を Markdown 形式で出力するヘルパーメソッド。Before フィールドが存在する場合、そのフィールドのタイプとラベルを Markdown 形式で表示します。例えば、"- **Before**: `SingleLineText` / Customer Name (customer_name)" のような形式で出力されます。Before フィールドが null の場合は何も出力しません。
    /// </summary>
    /// <param name="sb">StringBuilder インスタンス</param>
    /// <param name="diff">DiffResult インスタンス</param>
    private static void AppendBefore(StringBuilder sb, KintoneMetadataDiff diff) {
        if (diff.Before == null) { return; }

        sb.AppendLine($"- **Before**: `{diff.Before.FieldType}` / {diff.Before.GetLabelWithCode()}");
    }
    /// <summary>
    /// DiffResult の After フィールドの情報を Markdown 形式で出力するヘルパーメソッド。After フィールドが存在する場合、そのフィールドのタイプとラベルを Markdown 形式で表示します。例えば、"- **After**: `SingleLineText` / Customer Name (customer_name)" のような形式で出力されます。After フィールドが null の場合は何も出力しません。
    /// </summary>
    /// <param name="sb">StringBuilder インスタンス</param>
    /// <param name="diff">DiffResult インスタンス</param>
    private static void AppendAfter(StringBuilder sb, KintoneMetadataDiff diff) {
        if (diff.After == null) { return; }

        sb.AppendLine($"- **After**: `{diff.After.FieldType}` / {diff.After.GetLabelWithCode()}");
    }

    /// <summary>
    /// DiffResult の ChangedProperties を Markdown 形式で出力するヘルパーメソッド。変更されたプロパティがある場合、"**Changed Properties:**" という見出しの下に、変更されたプロパティのリストを Markdown 形式で表示します。例えば、"- **Changed Properties:**\n  - `FieldLabel`\n  - `Required`" のような形式で出力されます。変更されたプロパティがない場合は何も出力しません。
    /// </summary>
    /// <param name="sb">StringBuilder インスタンス</param>
    /// <param name="diff">DiffResult インスタンス</param>
    private static void AppendChangedProperties(StringBuilder sb, KintoneMetadataDiff diff) {
        if (diff.ChangedProperties.Count == 0) { return; }

        sb.AppendLine("- **Changed Properties:**");
        foreach (var p in diff.ChangedProperties) {
            sb.AppendLine($"  - `{p}`");
        }
    }

    /// <summary>
    /// テキストを指定したスペース数だけインデントするヘルパーメソッド。テキストの各行の先頭に、指定されたスペース数だけ空白を追加します。これにより、Markdown 形式でサブテーブルの差分を表示する際に、適切なインデントを付けることができます。例えば、Indent("Hello\nWorld", 4) と呼び出すと、"    Hello\n    World" という文字列が返されます。
    /// </summary>
    /// <param name="text">インデントするテキスト</param>
    /// <param name="spaces">インデントするスペースの数</param>
    /// <returns>インデントされたテキスト</returns>
    private static string Indent(string text, int spaces) {
        var pad = new string(' ', spaces);
        return pad + text.Replace("\n", "\n" + pad);
    }

    /// <summary>
    /// DiffResult のフィールド名を取得する
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    private static string GetFieldName(this KintoneMetadataDiff diff) => diff.After?.FieldCode ?? diff.Before?.FieldCode ?? "(unknown)";
}
