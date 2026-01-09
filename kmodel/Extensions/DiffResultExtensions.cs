using System.Text;
using System.Text.Json;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Enums;
using kmodel.Models;

namespace kmodel.Extensions;

// コメントは日本語で作成してください
/// <summary>
/// DiffResult 拡張メソッド
/// </summary>
public static class DiffResultExtensions {
    /// <summary>
    /// DiffResult をテキスト形式で出力する
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    public static string ToText(this DiffResult diff) {
        var sb = new StringBuilder();

        sb.AppendLine($"Diff Summary (Rev {diff.BeforeRevision} → Rev {diff.AfterRevision})");
        sb.AppendLine($"[{diff.DiffType}] {diff.GetFieldName()}");

        if (diff.Before != null) {
            sb.AppendLine($"  Before: {diff.Before.Type} / {diff.Before.GetLabelWithCode()}");
        }

        if (diff.After != null) {
            sb.AppendLine($"  After : {diff.After.Type} / {diff.After.GetLabelWithCode()}");
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
    private static string GetLabelWithCode(this KintoneFieldMetadata field) {
        var label = field.Label ?? "(no label)";
        return $"{label} ({field.Code})";
    }

    /// <summary>
    /// DiffResult を JSON 形式で出力する
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    public static string ToJson(this DiffResult diff) {
        return JsonSerializer.Serialize(diff, new JsonSerializerOptions {
            WriteIndented = true
        });
    }

    /// <summary>
    /// DiffResult を Markdown 形式で出力する
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    public static string ToMarkdown(this DiffResult diff) {
        var sb = new StringBuilder();

        // タイトル行
        sb.AppendLine($"## Diff Summary (Rev {diff.BeforeRevision} → {diff.AfterRevision})");
        sb.AppendLine($"### {diff.DiffType}: `{diff.GetFieldName()}`");
        sb.AppendLine();

        // DiffTypeごとの出力
        switch (diff.DiffType) {
            case DiffTypes.Added:
                AppendAfter(sb, diff);
                break;

            case DiffTypes.Removed:
                AppendBefore(sb, diff);
                break;

            case DiffTypes.Changed:
            case DiffTypes.ChangedOptions:
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
    private static void AppendBefore(StringBuilder sb, DiffResult diff) {
        if (diff.Before == null) { return; }

        sb.AppendLine($"- **Before**: `{diff.Before.Type}` / {diff.Before.GetLabelWithCode()}");
    }

    private static void AppendAfter(StringBuilder sb, DiffResult diff) {
        if (diff.After == null) { return; }

        sb.AppendLine($"- **After**: `{diff.After.Type}` / {diff.After.GetLabelWithCode()}");
    }

    private static void AppendChangedProperties(StringBuilder sb, DiffResult diff) {
        if (diff.ChangedProperties.Count == 0) { return; }

        sb.AppendLine("- **Changed Properties:**");
        foreach (var p in diff.ChangedProperties) {
            sb.AppendLine($"  - `{p}`");
        }
    }

    private static string Indent(string text, int spaces) {
        var pad = new string(' ', spaces);
        return pad + text.Replace("\n", "\n" + pad);
    }

    /// <summary>
    /// DiffResult のフィールド名を取得する
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    private static string GetFieldName(this DiffResult diff) => diff.After?.Code ?? diff.Before?.Code ?? "(unknown)";
}
