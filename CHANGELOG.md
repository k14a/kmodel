# Changelog

すべての重要な変更はこのファイルに記録します。
フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づき、バージョニングは [Semantic Versioning](https://semver.org/lang/ja/) に準拠します。

---

## [0.9.1] - 2026-06-09

### ⚠️ 破壊的変更

- `kmodel.config.json` の設定構造を変更しました。C#/Python/ファイル出力オプションがフラット構造からネスト構造に移行しました（[DES-2]）。

  **移行前:**
  ```jsonc
  {
    "generates": [{
      "namespace": "KintoneModels.App111",
      "useRecord": false,
      "outputDirectory": "./Models/App111"
    }]
  }
  ```

  **移行後:**
  ```jsonc
  {
    "generates": [{
      "csharp": {
        "namespace": "KintoneModels.App111",
        "useRecord": false
      },
      "fileWriter": {
        "outputDirectory": "./Models/App111"
      }
    }]
  }
  ```

### Added

- **テストプロジェクト** `kmodel.Tests` を新規追加（xUnit、57テスト）。
  - [TEST-1] `EmitterOptionsConverter` — C#/Python 向けのプロパティマッピング検証（C# 10項目・Python 4項目・未対応言語例外）
  - [TEST-2] `GenerateRunner.ParseDelimitedLine` / `Escape` — CSV/TSV パーサーのクォート・エスケープ処理検証
  - [TEST-3] `KintoneMetadataDiffExtensions.ToText` / `ToMarkdown` — Added/Removed/Changed/ChangedOptions 各フォーマット検証
  - [TEST-4] `FileWriter` — 上書き制御・ファイル名検証・`{appId}` プレースホルダー置換検証
  - [TEST-5] `ConfigLoader.Load` — camelCase/enum 文字列/コメントスキップ/末尾カンマ/エラー系検証
  - [TEST-6] `*Config.ToOptions()` — `GenerateConfig` / `RawConfig` / `DiffConfig` の全プロパティマッピング検証

- **`ILocalSchemaProviderFactory`** インターフェースおよび **`LocalSchemaProviderFactory`** 実装クラスを追加（[CA-2]）。`GenerateRunner` が `LocalJsonSchemaProvider` を直接 `new` していた問題を解消。

- **`IEmitterOptionsConverter`** インターフェースおよび **`EmitterOptionsConverter`** 実装クラスを追加（[NM-2]）。旧 `ISchemaConverter` / `SchemaConverter` を責務名に合わせて改名。

- **`CSharpOptions`** / **`PythonOptions`** / **`FileWriterOptions`** クラスを追加（[DES-2]）。`GenerateOptions` から言語固有オプションとファイル出力オプションを分離。

- **`GenerateConfig`** / **`RawConfig`** / **`DiffConfig`** クラスを追加（[CA-3]）。設定ファイルモデルと実行時オプションを分離し、`ToOptions()` 変換メソッドを提供。

- **`Properties/AssemblyInfo.cs`** を追加。`[assembly: InternalsVisibleTo("kmodel.Tests")]` を定義し、`internal` メソッドのテストを可能にした。

### Changed

- [DES-2] `GenerateOptions` を整理。言語固有オプション（`CSharp.*`, `Python.*`）とファイル出力オプション（`FileWriter.*`）をネスト構造に変更（**破壊的変更** — 上記参照）。

- [CA-3] `KModelConfig` が `GenerateOptions` / `RawOptions` / `DiffOptions` を直接保持していた構造を変更。専用の Config クラス（`GenerateConfig` 等）に分離し、`KModelApplication` で `ToOptions()` により変換するように改修。

- [CA-2] `GenerateRunner` の `LocalJsonSchemaProvider` 直接生成を廃止。`ILocalSchemaProviderFactory` 経由に変更し、`_loggerFactory` フィールドも不要になったため削除（[UNUSED-6]）。

- [NM-2] `ISchemaConverter` / `SchemaConverter` を `IEmitterOptionsConverter` / `EmitterOptionsConverter` に改名。

- [UP-1] `KintoneNetLibrary` 0.9.0 → 0.9.1 の破壊的変更に追従。
  - `ICodeEmitterFactory.Create(lang, nameConverter)` → `Create(lang)`
  - `ICodeEmitter.Emit(schema, options)` → `Emit(schema, options, nameConverter)`

- [NM-1] `GenerateCommandBuilder` の `SetHandler` 内で `host` を直接キャプチャしていた箇所を `this._host` に統一。

- [NM-3] `StaticLogMessages.MethodStart` / `MethodFinish` の無視されていた `message` パラメータを削除。

- [DES-3] `FileWriter.WriteFile` の空ヘッダー生成コード（常に空文字列の `StringBuilder`）を削除。

- [DES-4] `Program.cs` の `finally` ブロックで "kmodel finished." が重複ログされていた問題を修正。

- [DES-5] `DiffRunner` / `RawRunner` の `SetDomain` と `ApiTokenAccess` の冗長な呼び出しを整理。

- [TEST-2] `GenerateRunner.ParseDelimitedLine` / `Escape` のアクセス修飾子を `private static` → `internal static` に変更。

### Removed

- `ISchemaConverter` インターフェース（[NM-2] — `IEmitterOptionsConverter` に改名）。
- `SchemaConverter` クラス（[NM-2] — `EmitterOptionsConverter` に改名）。
- `GenerateOptionsExtensions` クラス（[CA-3] — Config/Options 分離により不要化）。
- `IFileWriter.WriteRawMetadata` メソッド（[DES-1] — 未実装のまま公開されていたため削除。`RawRunner` が直接 `File.WriteAllTextAsync` を呼ぶよう整理）。
- `KintoneMetadataDiffExtensions.ToJson` 拡張メソッド（[UNUSED-4] — 未使用のため削除）。

### Fixed

- [UNUSED-1] `Program.cs` の `configLoader` 未使用変数を削除。
- [UNUSED-2] `IMetadataConverter` の DI 二重登録を修正。
- [UNUSED-3] `IKintoneFieldParser` の DI 二重登録を修正。
- [UNUSED-5] `kmodel.csproj` の `TreatWarningsAsErrors` 重複宣言を修正。

---

## [0.9.0] 以前

このファイルは v0.9.1 から記録を開始しています。それ以前の変更履歴は git ログを参照してください。
