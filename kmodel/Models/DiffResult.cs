using KintoneNetLibrary.Domain.Entities;
using kmodel.Enums;

namespace kmodel.Models;

/// <summary>
/// Diff 結果モデル
/// </summary>
public class DiffResult {
    /// <summary>
    /// 変更内容
    /// </summary>
    public DiffTypes DiffType { get; set; }
    /// <summary>
    /// 変更前(Addedの場合はnull)
    /// </summary>
    public KintoneFieldMetadata? Before { get; set; }
    /// <summary>
    /// 変更後(Removedの場合はnull)
    /// </summary>
    public KintoneFieldMetadata? After { get; set; }
    /// <summary>
    /// 変更対象フィールド
    /// </summary>
    public List<string> ChangedProperties { get; set; } = [];
    /// <summary>
    /// サブテーブルの差分リスト
    /// </summary>
    public List<DiffResult> SubTableDiffs { get; set; } = [];
    /// <summary>
    /// 変更前のリビジョン番号
    /// </summary>
    public int? BeforeRevision { get; set; }
    /// <summary>
    /// 変更後のリビジョン番号
    /// </summary>
    public int? AfterRevision { get; set; }
}