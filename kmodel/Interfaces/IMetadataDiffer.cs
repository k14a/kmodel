using KintoneNetLibrary.Domain.Entities;
using kmodel.Models;

namespace kmodel.Interfaces;

/// <summary>
/// メタデータ差分比較インターフェース
/// </summary>
public interface IMetadataDiffer {
    /// <summary>
    /// 2つのメタデータを比較し、差分を取得する
    /// </summary>
    /// <param name="previous">以前のメタデータ</param>
    /// <param name="latest">最新のメタデータ</param>
    /// <returns>差分のリスト</returns>
    List<DiffResult> Compare(KintoneAppMetadata previous, KintoneAppMetadata latest);
}