# AppId: 9432
# Revision: 34
# module: kintone_models
# AppId: 9432
# Revision: 34

@dataclass
class App9432:
    # Label: メールアドレス
    # FieldCode: Email
    email: str

    # Label: チェックボックス
    # FieldCode: Checkboxes
    # Options: ["チェック4", "チェック2", "チェック3", "チェック1"]
    checkboxes: list[str]

    # Label: 電話番号
    # FieldCode: Telephone
    telephone: str

    # Label: レイティング
    # FieldCode: Rating
    rating: int | None

    # Label: 追加テキストボックス
    # FieldCode: AddedTextBox
    added_text_box: str

    # Label: 複数選択
    # FieldCode: MultiSelector
    # Options: ["選択肢5", "選択肢4", "選択肢1", "選択肢3", "選択肢2"]
    multi_selector: list[str]

    # Label: リッチテキスト
    # FieldCode: RichText
    rich_text: str

    # Label: 分野
    # FieldCode: Classification
    # Options: ["SF", "技術書", "雑誌"]
    classification: str

    # Label: Webアドレス
    # FieldCode: WebAddress
    web_address: str

    # Label: UUID
    # FieldCode: UUID
    uuid: str

    # Label: 添付ファイル
    # FieldCode: Files
    files: list[File]

    # Label: 作業者
    # FieldCode: 作業者
    # TODO: フィールド名を変換できませんでした。手動で修正してください。
    __INVALID_FIELD_NAME__: str

    # Label: ユーザー選択
    # FieldCode: UserSelect
    user_select: list[User]

    # Label: タイトル
    # FieldCode: Title
    title: str

    # Label: 発行日
    # FieldCode: ReleaseDate
    release_date: datetime.datetime | None

    # Label: おすすめ度
    # FieldCode: Recommendation
    # Options: ["1", "2", "3", "4", "5"]
    recommendation: str

    # Label: グループ選択
    # FieldCode: GroupSelect
    group_select: list[Group]

    # Label: 時刻
    # FieldCode: TimeField
    time_field: datetime.time | None

    # Label: レビュー
    # FieldCode: Reviews
    reviews: str

    # Label: 組織選択
    # FieldCode: DivisionSelect
    division_select: list[Organization]

    # Label: 価格
    # FieldCode: Price
    price: int | None

    # Label: 日付
    # FieldCode: DateField
    date_field: datetime.date | None

@dataclass
class SubTableAppDetails:
    # Label: No
    # FieldCode: No
    no: int | None

    # Label: 取扱店
    # FieldCode: StoreName
    store_name: str

    # Label: 梱包状態
    # FieldCode: PackageType
    # Options: ["梱包する"]
    package_type: list[str]

    # Label: 配送日
    # FieldCode: DeliveryDate
    delivery_date: datetime.date | None

