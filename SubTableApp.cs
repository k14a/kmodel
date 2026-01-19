// AppId: 9432
// Revision: 34
using System;
using KintoneNetLibrary.Domain.Entities;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Models;

    /// <summary>
    /// サブテーブル: サブテーブル
    /// </summary>
public partial class SubTableApp : KintoneSubTableBase
{
    /// <summary>
    /// No
    /// </summary>
    [KintoneItem(FieldCode = "No")]
    public int | None field_3a1aba { get; set; }

    /// <summary>
    /// 取扱店
    /// </summary>
    [KintoneItem(FieldCode = "StoreName")]
    public str field_60df57 { get; set; }

    /// <summary>
    /// 梱包状態
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - 梱包する
    /// </values>
    [KintoneItem(FieldCode = "PackageType")]
    public list[str] field_ed199f { get; set; }

    /// <summary>
    /// 配送日
    /// </summary>
    [KintoneItem(FieldCode = "DeliveryDate")]
    public datetime.date | None field_2156eb { get; set; }

}
