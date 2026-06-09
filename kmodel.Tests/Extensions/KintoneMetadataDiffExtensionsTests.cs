using kmodel.Extensions;
using KintoneNetLibrary.Domain.Entities;
using KintoneNetLibrary.Domain.Enums;

namespace kmodel.Tests.Extensions;

public class KintoneMetadataDiffExtensionsTests {

    private static KintoneMetadataDiff CreateDiff(
        KintoneMetadataDiffTypes diffType,
        KintoneFieldMetadata? before = null,
        KintoneFieldMetadata? after = null,
        int? beforeRevision = 1,
        int? afterRevision = 2,
        List<string>? changedProperties = null,
        List<KintoneMetadataDiff>? subTableDiffs = null) {
        return new KintoneMetadataDiff {
            DiffType = diffType,
            Before = before,
            After = after,
            BeforeRevision = beforeRevision,
            AfterRevision = afterRevision,
            ChangedProperties = changedProperties ?? [],
            SubTableDiffs = subTableDiffs ?? [],
        };
    }

    private static KintoneFieldMetadata CreateField(
        string fieldCode,
        string? fieldLabel = "TestLabel",
        KintoneFieldType fieldType = KintoneFieldType.SingleLineText) {
        return new KintoneFieldMetadata {
            FieldCode = fieldCode,
            FieldLabel = fieldLabel!,
            FieldType = fieldType,
        };
    }

    // ---------------------------------------------------------------
    // ToText
    // ---------------------------------------------------------------

    [Fact]
    public void ToText_Added_AfterのみBeforeなし() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("field1", "フィールド1"));

        var result = diff.ToText();

        Assert.Contains("[Added] field1", result);
        Assert.Contains("After : SingleLineText / フィールド1 (field1)", result);
        Assert.DoesNotContain("Before:", result);
    }

    [Fact]
    public void ToText_Removed_BeforeのみAfterなし() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Removed,
            before: CreateField("field1", "フィールド1"));

        var result = diff.ToText();

        Assert.Contains("[Removed] field1", result);
        Assert.Contains("Before: SingleLineText / フィールド1 (field1)", result);
        Assert.DoesNotContain("After :", result);
    }

    [Fact]
    public void ToText_Changed_BeforeとAfterの両方が出力される() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Changed,
            before: CreateField("field1", "旧ラベル"),
            after: CreateField("field1", "新ラベル"));

        var result = diff.ToText();

        Assert.Contains("[Changed] field1", result);
        Assert.Contains("Before: SingleLineText / 旧ラベル (field1)", result);
        Assert.Contains("After : SingleLineText / 新ラベル (field1)", result);
    }

    [Fact]
    public void ToText_ChangedPropertiesあり_プロパティ一覧が出力される() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Changed,
            before: CreateField("f1"),
            after: CreateField("f1"),
            changedProperties: ["FieldLabel", "Required"]);

        var result = diff.ToText();

        Assert.Contains("Changed Properties:", result);
        Assert.Contains("- FieldLabel", result);
        Assert.Contains("- Required", result);
    }

    [Fact]
    public void ToText_FieldLabelがnull_noLabelと表示される() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("field1", null));

        var result = diff.ToText();

        Assert.Contains("(no label) (field1)", result);
    }

    [Fact]
    public void ToText_SubTableDiffsあり_サブテーブル差分が含まれる() {
        var subDiff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("sub_field1", "サブフィールド"));
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Changed,
            before: CreateField("table1"),
            after: CreateField("table1"),
            subTableDiffs: [subDiff]);

        var result = diff.ToText();

        Assert.Contains("SubTable Diffs:", result);
        Assert.Contains("[Added] sub_field1", result);
    }

    [Fact]
    public void ToText_BeforeとAfterが両方null_unknownが表示される() {
        var diff = CreateDiff(KintoneMetadataDiffTypes.Added);

        var result = diff.ToText();

        Assert.Contains("(unknown)", result);
    }

    [Fact]
    public void ToText_リビジョン番号がタイトルに含まれる() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("f1"),
            beforeRevision: 3,
            afterRevision: 4);

        var result = diff.ToText();

        Assert.Contains("Rev 3 → Rev 4", result);
    }

    // ---------------------------------------------------------------
    // ToMarkdown
    // ---------------------------------------------------------------

    [Fact]
    public void ToMarkdown_Added_AfterのみBeforeなし() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("field1", "フィールド1"));

        var result = diff.ToMarkdown();

        Assert.Contains("### Added: `field1`", result);
        Assert.Contains("- **After**: `SingleLineText`", result);
        Assert.DoesNotContain("**Before**:", result);
    }

    [Fact]
    public void ToMarkdown_Removed_BeforeのみAfterなし() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Removed,
            before: CreateField("field1", "フィールド1"));

        var result = diff.ToMarkdown();

        Assert.Contains("### Removed: `field1`", result);
        Assert.Contains("- **Before**: `SingleLineText`", result);
        Assert.DoesNotContain("**After**:", result);
    }

    [Fact]
    public void ToMarkdown_Changed_BeforeAfterChangedPropertiesが含まれる() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Changed,
            before: CreateField("field1", "旧ラベル"),
            after: CreateField("field1", "新ラベル"),
            changedProperties: ["FieldLabel"]);

        var result = diff.ToMarkdown();

        Assert.Contains("### Changed: `field1`", result);
        Assert.Contains("- **Before**:", result);
        Assert.Contains("- **After**:", result);
        Assert.Contains("- **Changed Properties:**", result);
        Assert.Contains("  - `FieldLabel`", result);
    }

    [Fact]
    public void ToMarkdown_ChangedOptions_Changed同様に出力される() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.ChangedOptions,
            before: CreateField("field1"),
            after: CreateField("field1"),
            changedProperties: ["Options"]);

        var result = diff.ToMarkdown();

        Assert.Contains("### ChangedOptions: `field1`", result);
        Assert.Contains("- **Before**:", result);
        Assert.Contains("- **After**:", result);
        Assert.Contains("- **Changed Properties:**", result);
    }

    [Fact]
    public void ToMarkdown_SubTableDiffsあり_インデントされて含まれる() {
        var subDiff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("sub_field1"));
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Changed,
            before: CreateField("table1"),
            after: CreateField("table1"),
            subTableDiffs: [subDiff]);

        var result = diff.ToMarkdown();

        Assert.Contains("**SubTable Diffs:**", result);
        // Indent(sub.ToMarkdown(), 2) で 2 スペースのインデントが付加される
        Assert.Contains("  ## Diff Summary", result);
    }

    [Fact]
    public void ToMarkdown_リビジョン番号がタイトルに含まれる() {
        var diff = CreateDiff(
            KintoneMetadataDiffTypes.Added,
            after: CreateField("f1"),
            beforeRevision: 5,
            afterRevision: 6);

        var result = diff.ToMarkdown();

        Assert.Contains("Rev 5 → 6", result);
    }
}
