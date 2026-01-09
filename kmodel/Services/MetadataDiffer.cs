using KintoneNetLibrary.Domain.Entities;
using kmodel.Interfaces;
using kmodel.Models;

namespace kmodel.Services;

/// <summary>
/// メタデータ差分比較サービス
/// </summary>
public class MetadataDiffer : IMetadataDiffer {
    /// <summary>
    /// 2つのメタデータを比較し、差分を取得する
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="latest"></param>
    /// <returns></returns>
    public List<DiffResult> Compare(KintoneAppMetadata previous, KintoneAppMetadata latest) {
        var diffs = new List<DiffResult>();

        // 追加されたフィールド
        foreach (var f in latest.Fields.Where(f => previous.Fields.All(pf => pf.Code != f.Code))) {
            diffs.Add(new DiffResult {
                DiffType = Enums.DiffTypes.Added,
                After = f
            });
        }

        // 削除されたフィールド
        foreach (var f in previous.Fields.Where(f => latest.Fields.All(lf => lf.Code != f.Code))) {
            diffs.Add(new DiffResult {
                DiffType = Enums.DiffTypes.Removed,
                Before = f
            });
        }

        // タイプ変更
        foreach (var latestField in latest.Fields) {
            var prevField = previous.Fields.FirstOrDefault(pf => pf.Code == latestField.Code);
            if (prevField == null) { continue; }

            var changedProps = new List<string>();

            if (prevField.Type != latestField.Type) { changedProps.Add("Type"); }
            if (prevField.Label != latestField.Label) { changedProps.Add("Label"); }
            if (prevField.Required != latestField.Required) { changedProps.Add("Required"); }
            // オプションの比較（単純なリスト比較）
            if (!Enumerable.SequenceEqual(prevField.Options ?? [], latestField.Options ?? [])) {
                changedProps.Add("Options");
            }
            if (changedProps.Count > 0) {
                diffs.Add(new DiffResult {
                    DiffType = Enums.DiffTypes.ChangedOptions,
                    Before = prevField,
                    After = latestField,
                    ChangedProperties = changedProps
                });
            }
        }

        return diffs;
    }
}