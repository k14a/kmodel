# コードレビュー TODO

## 対応順序一覧

影響範囲・リスクの小さいものから、設計変更を伴う大きなものへ段階的に対応する順序。

| 優先度 | ID | タイトル | 分類 | 工数感 |
|:---:|:---|:---|:---|:---:|
| 1 | [UNUSED-5] | `csproj` の `TreatWarningsAsErrors` 重複 | 重複コード | 小 |
| 2 | [UNUSED-1] | `Program.cs` の `configLoader` 未使用変数 | 未使用コード | 小 |
| 3 | [UNUSED-2] | `IMetadataConverter` 二重登録 | 重複コード | 小 |
| 4 | [UNUSED-3] | `IKintoneFieldParser` 二重登録 | 重複コード | 小 |
| 5 | [NM-1] | `GenerateCommandBuilder` が `host` を直接キャプチャ | 命名・一貫性 | 小 |
| 6 | [DES-4] | `finally` ブロックで "kmodel finished." が重複ログ | 設計 | 小 |
| 7 | [NM-3] | `MethodStart`/`MethodFinish` の `message` パラメータが無視される | 命名・一貫性 | 小 |
| 8 | [DES-3] | `FileWriter.WriteFile` のヘッダー生成コードが空 | 設計 | 小 |
| 9 | [UNUSED-4] | `ToJson` 拡張メソッドが未使用 | 未使用コード | 小 |
| 10 | [DES-1] | `IFileWriter.WriteRawMetadata` が未実装 | 設計 | 中 |
| 11 | [DES-5] | `SetDomain` と `ApiTokenAccess` の冗長呼び出し | 設計 | 中 |
| 12 | [NM-2] | `ISchemaConverter` の名前が責務と乖離 | 命名・一貫性 | 中 |
| 13 | [CA-2] | `GenerateRunner` が `LocalJsonSchemaProvider` を直接 `new` | Clean Architecture | 中 |
| 14 | [UNUSED-6] | `_loggerFactory` フィールドが肥大化（CA-2 依存） | 未使用コード | 中 |
| 15 | [DES-2] | `GenerateOptions` のゴッドクラス問題 | 設計 | 大 |
| 16 | [CA-3] | `KModelConfig` が CLI オプションクラスを直接参照 | Clean Architecture | 大 |
| 17 | [CA-1] | `IFileWriter` が外部ライブラリのドメインモデルに依存 | Clean Architecture | 大 |
| 18 | [UP-1] | `ICodeEmitterFactory.Create` シグネチャ変更への追従 | ライブラリ更新 | 小 |

### テストケース

テスト対象の優先度順。Runner 系・CommandBuilder 系は外部依存が多くコスト対効果が低いため対象外とする。

| 優先度 | ID | タイトル | 備考 | 工数感 |
|:---:|:---|:---|:---|:---:|
| 19 | [TEST-1] | `EmitterOptionsConverter` の変換ロジック | 外部依存なし・NullLogger で完結 | 小 |
| 20 | [TEST-2] | `GenerateRunner` の CSV/TSV パーサー（`ParseDelimitedLine` / `Escape`） | `private` → `internal` 化が必要 | 小 |
| 21 | [TEST-3] | `KintoneMetadataDiffExtensions.ToText` / `ToMarkdown` | 純粋関数・外部依存なし | 小 |
| 22 | [TEST-4] | `FileWriter` の上書き制御・ファイル名検証 | 一時ディレクトリを使用 | 中 |
| 23 | [TEST-5] | `ConfigLoader.Load` のデシリアライズ | 一時ファイルを使用 | 中 |
| 24 | [TEST-6] | `*Config.ToOptions()` のプロパティマッピング | リグレッション保護 | 小 |

---

## Clean Architecture 依存方向

### [CA-1] `IFileWriter` インターフェースが外部ライブラリのドメインモデルに依存している
- [x] 対応済み
- **場所**: [Interfaces/IFileWriter.cs](kmodel/Interfaces/IFileWriter.cs)
- **問題**: `IFileWriter` が `KintoneNetLibrary.CodeGen.Domain.Models.GeneratedSubTableModel` / `GeneratedHelperClass`、`KintoneNetLibrary.CodeGen.Domain.Options.CodeEmitterOptions` などに依存している。インターフェース定義が外部ライブラリのドメイン詳細に縛られており、差し替えが困難。
- **対応**: `IFileWriter` の引数を kmodel 側のモデルに変換してから渡すか、インターフェース定義をよりシンプルな値型で設計し直す。

### [CA-2] `GenerateRunner` が Infrastructure 実装を直接 `new` している
- [x] 対応済み
- **場所**: [Services/GenerateRunner.cs:79](kmodel/Services/GenerateRunner.cs#L79)
- **問題**: `new LocalJsonSchemaProvider(...)` を Application 層のサービスが直接 `new` している。Infrastructure の具象クラスに依存しており、依存が逆転していない。
- **対応**: `ILocalSchemaProviderFactory` などのファクトリインターフェースを導入して DI コンテナ側で解決する。

### [CA-3] `KModelConfig` が CLI オプションクラスを直接参照している
- [x] 対応済み
- **場所**: [Config/KModelConfig.cs](kmodel/Config/KModelConfig.cs)
- **問題**: 設定ファイルの構造を表す `KModelConfig` が、CLI 実行時オプションを兼ねる `GenerateOptions`/`RawOptions`/`DiffOptions` を直接含んでいる。Config 層と Application Options 層が混在している。
- **対応**: Config専用モデル(`GenerateConfig` 等)と実行時 Options を分離し、変換処理を設ける。

---

## 命名規則の一貫性

### [NM-1] `GenerateCommandBuilder` が `host` を直接キャプチャしている（他と不一致）
- [x] 対応済み
- **場所**: [Commands/GenerateCommandBuilder.cs:85](kmodel/Commands/GenerateCommandBuilder.cs#L85)
- **問題**: `SetHandler` 内で `host.Services.GetRequiredService<GenerateRunner>()` とコンストラクタパラメータの `host` をキャプチャしている。`DiffCommandBuilder` と `RawCommandBuilder` は `this._host` を使っており不一致。
- **対応**: `this._host` に統一する。

### [NM-2] `ISchemaConverter` の名前が責務と乖離している
- [x] 対応済み
- **場所**: [Interfaces/ISchemaConverter.cs](kmodel/Interfaces/ISchemaConverter.cs)、[Services/SchemaConverter.cs](kmodel/Services/SchemaConverter.cs)
- **問題**: 実際の責務は「`GenerateOptions` → `CodeEmitterOptions` への変換」であり、"Schema" という語は誤解を招く。
- **対応**: `IEmitterOptionsConverter` / `EmitterOptionsConverter` などへ改名する。

### [NM-3] `StaticLogMessages.MethodStart` / `MethodFinish` の `message` パラメータが実際には渡されない
- [x] 対応済み
- **場所**: [Logging/StaticLogMessages.cs:24](kmodel/Logging/StaticLogMessages.cs#L24), [L40](kmodel/Logging/StaticLogMessages.cs#L40)
- **問題**: `message` パラメータを受け取っているが、内部の `MethodStartInner` / `MethodFinishInner` に渡していない。パラメータが機能しているように見えて無視される。
- **対応**: `message` パラメータを削除するか、内部メソッドに渡すよう修正する。

---

## 設計上の問題

### [DES-1] `IFileWriter.WriteRawMetadata` が未実装のまま公開されている
- [x] 対応済み
- **場所**: [Services/FileWriter.cs:120](kmodel/Services/FileWriter.cs#L120)、[Interfaces/IFileWriter.cs:44](kmodel/Interfaces/IFileWriter.cs#L44)
- **問題**: `throw new NotImplementedException()` のまま。`RawRunner` は `IFileWriter` を使わず `File.WriteAllTextAsync` を直接呼んでいるため、このメソッドは死んでいる。インターフェース定義と実装が乖離している。
- **対応**: `IFileWriter` から `WriteRawMetadata` を削除し、`RawRunner` の責務として整理する。

### [DES-2] `GenerateOptions` がすべての言語固有オプションを混在させている（ゴッドクラス）
- [x] 対応済み
- **場所**: [Options/GenerateOptions.cs](kmodel/Options/GenerateOptions.cs)
- **問題**: C# 固有（`Namespace`, `UseRecord`, `UsePartial`, `NullableEnabled`）、Python 固有（`ModuleName`, `UseTypeHint`）、ファイル出力用（`OutputDirectory`, `OverwriteExistingFiles`）が同一クラスに混在。`FileWriterOptions` が別途存在するのに `GenerateOptions` にも同じプロパティがある。
- **対応**: 言語固有オプションを継承または集約で切り出す。

### [DES-3] `FileWriter.WriteFile` のヘッダー生成コードが空
- [x] 対応済み
- **場所**: [Services/FileWriter.cs:155-158](kmodel/Services/FileWriter.cs#L155-L158)
- **問題**: `var header = new StringBuilder(); var finalCode = header.ToString() + code;` — `header` は常に空文字列であり、変数として存在する意味がない。コードの意図が不明確。
- **対応**: ヘッダー生成を実装するか、使わないなら削除する。

### [DES-4] `Program.cs` の `finally` ブロックで "kmodel finished." が重複ログされる
- [x] 対応済み
- **場所**: [Program.cs:39-53](kmodel/Program.cs#L39-L53)
- **問題**: `args.Length == 0` のとき L41 の `logger.LogDebug("kmodel finished.")` と、`finally` ブロックの L52 の同じメッセージの両方が実行される。
- **対応**: `finally` のログを削除し、`args.Length == 0` パスのログのみ残す。

### [DES-5] `DiffRunner` で `SubDomain` を `SetDomain` した後すぐ `ApiTokenAccess` でも使う冗長な呼び出し
- [x] 対応済み
- **場所**: [Services/DiffRunner.cs:35-36](kmodel/Services/DiffRunner.cs#L35-L36), [Services/RawRunner.cs:27-29](kmodel/Services/RawRunner.cs#L27-L29)
- **問題**: `this._schemaProvider.SetDomain(subDomain)` を呼んだ後、`new ApiTokenAccess(subDomain, ...)` でも同じ `subDomain` を渡している。`SetDomain` の効果と `ApiTokenAccess` の役割が重複している可能性がある。
- **対応**: `SetDomain` と `ApiTokenAccess` の責務を整理し、どちらかに一本化する。

---

## 未使用コード・重複コード

### [UNUSED-1] `Program.cs` で `ConfigLoader` を取得しているが未使用
- [x] 対応済み
- **場所**: [Program.cs:33](kmodel/Program.cs#L33)
- **問題**: `var configLoader = host.Services.GetRequiredService<ConfigLoader>();` が宣言されているが、その後一切使われていない。`KModelApplication` が内部で `ConfigLoader` を直接注入されている。
- **対応**: 不要な変数を削除する。

### [UNUSED-2] `IMetadataConverter` が DI コンテナに二重登録されている
- [x] 対応済み
- **場所**: [Program.cs:88](kmodel/Program.cs#L88) と [Program.cs:99](kmodel/Program.cs#L99)
- **問題**: `services.AddTransient<IMetadataConverter, MetadataConverter>()` が2回登録されている。
- **対応**: 重複している1行を削除する。

### [UNUSED-3] `IKintoneFieldParser` が DI コンテナに二重登録されている
- [x] 対応済み
- **場所**: [Program.cs:91](kmodel/Program.cs#L91) と [Program.cs:105](kmodel/Program.cs#L105)
- **問題**: `services.AddTransient<IKintoneFieldParser, KintoneFieldParser>()` が2回登録されている。
- **対応**: 重複している1行を削除する。

### [UNUSED-4] `KintoneMetadataDiffExtensions.ToJson` が使われていない
- [x] 対応済み
- **場所**: [Extensions/KintoneMetadataDiffExtensions.cs:67](kmodel/Extensions/KintoneMetadataDiffExtensions.cs#L67)
- **問題**: `ToJson` 拡張メソッドが定義されているが、`FileWriter.WriteDiffResults` は独自に `JsonSerializer.Serialize` を呼んでおり、この拡張メソッドを使っていない。
- **対応**: `FileWriter` で `ToJson()` を呼ぶよう統一するか、使わないなら削除する。

### [UNUSED-5] `csproj` で `TreatWarningsAsErrors` が重複宣言されている
- [x] 対応済み
- **場所**: [kmodel.csproj:9](kmodel/kmodel.csproj#L9) と [kmodel.csproj:12](kmodel/kmodel.csproj#L12)
- **問題**: `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>` が2箇所に記載されている。
- **対応**: 1箇所に統一する。

### [UNUSED-6] `GenerateRunner` の `_loggerFactory` フィールドの依存が肥大化
- [x] 対応済み
- **場所**: [Services/GenerateRunner.cs:42](kmodel/Services/GenerateRunner.cs#L42)
- **問題**: `ILoggerFactory` を注入しているのは `LocalJsonSchemaProvider` のロガーを作るためだけ。`LocalJsonSchemaProvider` の直接生成([CA-2])を解消すれば不要になる。
- **対応**: [CA-2] の対応後に削除する。

---

## KintoneNetLibrary 0.9.1 対応

KintoneNetLibrary を 0.9.0 → 0.9.1 へバージョンアップしたことによる破壊的変更への追従。
変更コミット: `5fbdc23`（7-B-2: ICodeEmitter/ISubTableEmitter の nameConverter を Emit 引数に移動し SetNameConverter を削除）

### [UP-1] `ICodeEmitterFactory.Create` と `ICodeEmitter.Emit` のシグネチャ変更
- [x] 対応済み
- **場所**: [Services/GenerateRunner.cs](kmodel/kmodel/Services/GenerateRunner.cs) — `GenerateCodeAsync` メソッド内
- **問題**: 0.9.1 で `nameConverter` の受け渡し場所が変更された。
  - `ICodeEmitterFactory.Create(lang, nameConverter)` → `Create(lang)`（第2引数を削除）
  - `ICodeEmitter.Emit(schema, options)` → `Emit(schema, options, nameConverter)`（第3引数を追加）
  - 旧 `SetNameConverter()` メソッドは削除済み
- **変更前（kmodel 現在のコード）**:
  ```csharp
  var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage, nameConverter);
  var result = codeEmitter.Emit(schema, emitterOptions);
  ```
- **変更後（対応後のコード）**:
  ```csharp
  var codeEmitter = this._codeEmitterFactory.Create(options.GenerateLanguage);
  var result = codeEmitter.Emit(schema, emitterOptions, nameConverter);
  ```
- **対応**: 上記2行を修正するだけで現行の2ビルドエラーが解消される。

---

## テストケース

Runner 系（`GenerateRunner` / `RawRunner` / `DiffRunner`）と CommandBuilder 系は外部依存が多くコスト対効果が低いため対象外とする。

### [TEST-1] `EmitterOptionsConverter` の変換ロジック
- [x] 対応済み
- **場所**: [Services/EmitterOptionsConverter.cs](kmodel/Services/EmitterOptionsConverter.cs)
- **理由**: C#/Python 双方で多数のプロパティマッピングがある。DES-2 でネスト構造に変更したため、手動確認が難しいリグレッションが入り込みやすい。
- **テスト内容**:
  - C# 向け変換: `CSharp.*` プロパティが `CSharpEmitterOptions` へ正しくマッピングされること
  - Python 向け変換: `Python.*` プロパティが `PythonEmitterOptions` へ正しくマッピングされること
  - 未サポート言語指定時に `NotSupportedException` がスローされること

### [TEST-2] `GenerateRunner` の CSV/TSV パーサー
- [x] 対応済み
- **場所**: [Services/GenerateRunner.cs](kmodel/Services/GenerateRunner.cs) — `ParseDelimitedLine` / `Escape`
- **理由**: クォート・エスケープ処理は複雑で、エッジケースでバグが入りやすい。現状 `private` のため `internal` 化 + `[InternalsVisibleTo]` が前提となる。
- **テスト内容**:
  - 通常フィールド（クォートなし）
  - ダブルクォートで囲まれたフィールド
  - ダブルクォートのエスケープ（`""`）
  - 区切り文字を含むフィールド
  - 空フィールド
  - `Escape`: 区切り文字・改行・クォートを含む値のエスケープ

### [TEST-3] `KintoneMetadataDiffExtensions.ToText` / `ToMarkdown`
- [x] 対応済み
- **場所**: [Extensions/KintoneMetadataDiffExtensions.cs](kmodel/Extensions/KintoneMetadataDiffExtensions.cs)
- **理由**: テキスト出力フォーマットは純粋関数で外部依存なし。Added/Removed/Changed の各 DiffType で出力形式が異なるため、リグレッション保護として有効。
- **テスト内容**:
  - Added / Removed / Changed / ChangedOptions 各 DiffType での出力形式
  - SubTableDiffs が存在する場合のインデント
  - Before / After が null の場合の出力

### [TEST-4] `FileWriter` の上書き制御・ファイル名検証
- [x] 対応済み
- **場所**: [Services/FileWriter.cs](kmodel/Services/FileWriter.cs)
- **理由**: `OverwriteExistingFiles = false` 時の保護と不正ファイル名のガードは動作保証が必要。一時ディレクトリを使えば自動化可能。
- **テスト内容**:
  - 上書き禁止時: 既存ファイルをスキップすること
  - 上書き許可時: 既存ファイルを上書きすること
  - 不正ファイル名（`Path.GetInvalidFileNameChars()` を含む）の拒否
  - `WriteDiffResults` の `{appId}` プレースホルダー置換

### [TEST-5] `ConfigLoader.Load` のデシリアライズ
- [x] 対応済み
- **場所**: [Config/ConfigLoader.cs](kmodel/Config/ConfigLoader.cs)
- **理由**: camelCase + `JsonStringEnumConverter` の組み合わせ。設定ファイルのデシリアライズは実際に動かすまで分からない挙動がある。一時ファイルを使えば自動化可能。
- **テスト内容**:
  - 正常な JSON のデシリアライズ（camelCase キー・enum 文字列）
  - コメント付き JSON のスキップ（`ReadCommentHandling.Skip`）
  - ファイル未存在時の `FileNotFoundException`
  - デシリアライズ失敗時の `InvalidOperationException`

### [TEST-6] `*Config.ToOptions()` のプロパティマッピング
- [x] 対応済み
- **場所**: [Config/GenerateConfig.cs](kmodel/Config/GenerateConfig.cs)、[Config/RawConfig.cs](kmodel/Config/RawConfig.cs)、[Config/DiffConfig.cs](kmodel/Config/DiffConfig.cs)
- **理由**: CA-3 対応で導入した Config → Options の変換処理。プロパティを全量マッピングしており、将来プロパティが増えた際の漏れを検出するリグレッション保護として有効。
- **テスト内容**:
  - `GenerateConfig.ToOptions()`: 全プロパティが `GenerateOptions` に正しく反映されること
  - `RawConfig.ToOptions()`: 全プロパティが `RawOptions` に正しく反映されること
  - `DiffConfig.ToOptions()`: 全プロパティが `DiffOptions` に正しく反映されること
