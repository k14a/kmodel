// AppId: 9432
// Revision: 34
using System;
using KintoneNetLibrary.Domain.Entities;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Models;

public partial class App9432 : KintoneModelBase<App9432>
{
    /// <summary>
    /// メールアドレス
    /// </summary>
    [KintoneItem(FieldCode = "Email")]
    public string Email { get; set; }

    /// <summary>
    /// チェックボックス
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - チェック4
    /// - チェック2
    /// - チェック3
    /// - チェック1
    /// </values>
    [KintoneItem(FieldCode = "Checkboxes")]
    public List<string> Checkboxes { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    [KintoneItem(FieldCode = "Telephone")]
    public string Telephone { get; set; }

    /// <summary>
    /// レイティング
    /// </summary>
    [KintoneItem(FieldCode = "Rating")]
    public int? Rating { get; set; }

    /// <summary>
    /// 追加テキストボックス
    /// </summary>
    [KintoneItem(FieldCode = "AddedTextBox")]
    public string AddedTextBox { get; set; }

    /// <summary>
    /// 複数選択
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - 選択肢5
    /// - 選択肢4
    /// - 選択肢1
    /// - 選択肢3
    /// - 選択肢2
    /// </values>
    [KintoneItem(FieldCode = "MultiSelector")]
    public List<string> MultiSelector { get; set; }

    /// <summary>
    /// リッチテキスト
    /// </summary>
    [KintoneItem(FieldCode = "RichText")]
    public string RichText { get; set; }

    /// <summary>
    /// 分野
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - SF
    /// - 技術書
    /// - 雑誌
    /// </values>
    [KintoneItem(FieldCode = "Classification")]
    public string Classification { get; set; }

    /// <summary>
    /// Webアドレス
    /// </summary>
    [KintoneItem(FieldCode = "WebAddress")]
    public string WebAddress { get; set; }

    /// <summary>
    /// UUID
    /// </summary>
    [KintoneItem(FieldCode = "UUID")]
    public string UUID { get; set; }

    /// <summary>
    /// 添付ファイル
    /// </summary>
    [KintoneItem(FieldCode = "Files")]
    public List<KintoneFile> Files { get; set; }

    /// <summary>
    /// 作業者
    /// </summary>
    [KintoneItem(FieldCode = "作業者")]
    public string Assignee { get; set; }

    /// <summary>
    /// ユーザー選択
    /// </summary>
    [KintoneItem(FieldCode = "UserSelect")]
    public List<KintoneUser> UserSelect { get; set; }

    /// <summary>
    /// タイトル
    /// </summary>
    [KintoneItem(FieldCode = "Title")]
    public string Title { get; set; }

    /// <summary>
    /// 発行日
    /// </summary>
    [KintoneItem(FieldCode = "ReleaseDate")]
    public KintoneDateTime ReleaseDate { get; set; }

    /// <summary>
    /// おすすめ度
    /// </summary>
    /// <values>
    /// 選択肢:
    /// - 1
    /// - 2
    /// - 3
    /// - 4
    /// - 5
    /// </values>
    [KintoneItem(FieldCode = "Recommendation")]
    public string Recommendation { get; set; }

    /// <summary>
    /// グループ選択
    /// </summary>
    [KintoneItem(FieldCode = "GroupSelect")]
    public List<string> GroupSelect { get; set; }

    /// <summary>
    /// 時刻
    /// </summary>
    [KintoneItem(FieldCode = "TimeField")]
    public KintoneTimeOnly TimeField { get; set; }

    /// <summary>
    /// レビュー
    /// </summary>
    [KintoneItem(FieldCode = "Reviews")]
    public string Reviews { get; set; }

    /// <summary>
    /// 組織選択
    /// </summary>
    [KintoneItem(FieldCode = "DivisionSelect")]
    public List<string> DivisionSelect { get; set; }

    /// <summary>
    /// 価格
    /// </summary>
    [KintoneItem(FieldCode = "Price")]
    public int? Price { get; set; }

    /// <summary>
    /// 日付
    /// </summary>
    [KintoneItem(FieldCode = "DateField")]
    public KintoneDateOnly DateField { get; set; }

    /// <summary>
    /// サブテーブル: サブテーブル
    /// </summary>
    [KintoneItem(FieldCode = "Details", FieldType = KintoneFieldType.SubTable)]
    public List<SubTableDetails> Details { get; set; }

}
