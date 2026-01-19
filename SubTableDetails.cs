// AppId: 9432
// Revision: 34
using System;
using KintoneNetLibrary.Domain.Entities;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Models;

    /// <summary>
    /// サブテーブル: サブテーブル
    /// </summary>
public partial class SubTableDetails : KintoneSubTableBase
{
    /// <summary>
    /// No
    /// </summary>
    [KintoneItem(FieldCode = "No")]
    public str no { get; set; }

    /// <summary>
    /// 取扱店
    /// </summary>
    [KintoneItem(FieldCode = "StoreName")]
    public str storename { get; set; }

    /// <summary>
    /// 梱包状態
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - 梱包する
    /// </values>
    [KintoneItem(FieldCode = "PackageType")]
    public list[str] packagetype { get; set; }

    /// <summary>
    /// 配送日
    /// </summary>
    [KintoneItem(FieldCode = "DeliveryDate")]
    public str deliverydate { get; set; }

}
