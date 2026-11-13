# kmodel

Kintone の metadata.json / fields.json をシームレスに扱う CLI ツール。複数の言語（C#、Python、TypeScript、Go、Java）へのコード生成、raw metadata のエクスポート、メタデータの差分検出を支援します。

---

## 目次

1. [Introduction](#introduction)
2. [What's New](#whats-new)
3. [Features](#features)
4. [Installation](#installation)
5. [Quick Start](#quick-start)
6. [Configuration File](#configuration-file-kmodelconfigjson)
7. [CLI Commands](#cli-commands)
8. [Examples](#examples)
9. [Project Structure](#project-structure)
10. [Extending kmodel](#extending-kmodel)
11. [Contributing](#contributing)
12. [License](#license)

---

## Introduction

**kmodel** は、Kintone の API から取得したメタデータを活用し、以下の処理を自動化する CLI ツールです：

- **複数言語へのコード自動生成**: Kintone のアプリスキーマから、C#、Python、TypeScript、Go、Java のモデルコードを生成
- **Raw Metadata Export**: Kintone API から取得したメタデータを JSON 形式で保存
- **Metadata Diff**: 異なるバージョンのメタデータを比較し、変更内容を可視化
- **一括処理**: `kmodel.config.json` で複数アプリの設定を一元管理
- **System.CommandLine による直感的 CLI**: .NET の最新 CLI フレームワークで統一された使用感

kmodel は内部で Kintone API との通信およびメタデータ処理に
[KintoneNetLibrary](https://github.com/k14a/KintoneNetLibrary.git) を使用しています。

**対応言語**:
- C# (.NET)
- Python
- TypeScript
- Go
- Java

---

## What's New

### v0.9.1

**設計改善**

- `GenerateOptions` のゴッドクラス問題を解消。言語固有オプション（C#: `CSharpOptions`、Python: `PythonOptions`）とファイル出力オプション（`FileWriterOptions`）を別クラスに分離し、設定ファイルの構造がよりフラットになりました。
- Config モデルと実行時 Options を分離。`KModelConfig` が CLI オプションを直接参照する設計を解消し、`GenerateConfig` / `RawConfig` / `DiffConfig` と各 Options の間に変換層を導入しました。
- `ISchemaConverter` / `SchemaConverter` を `IEmitterOptionsConverter` / `EmitterOptionsConverter` に改名し、責務を明確化しました。
- `ILocalSchemaProviderFactory` を導入し、`GenerateRunner` が Infrastructure の具象クラスを直接 `new` する依存を解消しました。

**品質向上**

- xUnit テストプロジェクト `kmodel.Tests` を新設。57 件のユニットテストを追加しました。
  - `EmitterOptionsConverter` 変換ロジック（10 件）
  - `GenerateRunner` CSV/TSV パーサー（14 件）
  - `KintoneMetadataDiffExtensions.ToText` / `ToMarkdown`（14 件）
  - `FileWriter` の上書き制御・ファイル名検証（9 件）
  - `ConfigLoader.Load` のデシリアライズ（7 件）
  - `*Config.ToOptions()` プロパティマッピング（3 件）
- ビルド警告をすべて解消しました（CS9107 / CS8601 / CS8604 / CS1591）。

**⚠️ 設定ファイルの構造変更（破壊的変更）**

`generates` エントリの C#・Python 固有オプションおよびファイル出力オプションの JSON キーが変わりました。`kmodel.config.json` の更新が必要です。詳細は [Configuration File](#configuration-file-kmodelconfigjson) を参照してください。

---

## Features

### 主要機能

| 機能 | 説明 |
|---|---|
| **Code Generation** | Kintone のメタデータからモデルコードを複数言語で生成 |
| **Language Options** | C#、Python、TypeScript、Go、Java に対応 |
| **Pure Mode** | フレームワーク依存なし（C#）の純粋なモデルクラスを生成 |
| **Enum Generation** | ドロップダウン・ラジオボタンなどの選択肢を自動的に enum 化 |
| **Namespace/Module** | 言語に応じたモジュール・名前空間の自動設定 |
| **Name Table** | フィールドコードとプロパティ名の対応表でカスタムマッピングに対応 |
| **Metadata Export** | 生のメタデータを JSON で出力（API 応答をそのまま保存） |
| **Diff Reporting** | メタデータの変更を検出し、差分を複数形式でレポート |
| **Batch Processing** | `kmodel.config.json` で複数アプリを一度に処理 |
| **JSON Schema** | 設定ファイルの自動補完・バリデーション |
| **Extensible** | Emitter の追加で新言語対応が可能 |

---

## Installation

### 前提条件

- .NET 10.0 以上
- Windows、macOS、または Linux

### インストール方法

#### 1. リリース版からのインストール

GitHub Releases から最新版をダウンロードしてください。

```bash
# ダウンロード後、PATH に追加するか、直接実行
./kmodel --help
```

#### 2. ソースコードからのビルド

```bash
git clone https://github.com/k14a/kmodel.git
cd kmodel
dotnet build -c Release
cd kmodel
dotnet run -- --help
```

#### 3. グローバルツールとしてインストール

```bash
dotnet tool install -g kmodel
kmodel --help
```

### Dependencies

kmodel は以下のライブラリに依存しています：

- [KintoneNetLibrary](https://github.com/k14a/KintoneNetLibrary.git)  
  Kintone API との通信およびメタデータ処理を担当する .NET ライブラリです。

---

## Quick Start

### 1. 初期化（初めてのセットアップ）

```bash
kmodel init --lang CSharp
```

このコマンドは、カレントディレクトリに `kmodel.config.json` を作成します。

### 2. 設定ファイルを編集

エディタで `kmodel.config.json` を開き、以下を設定：

- `subDomain`: Kintone の完全なドメイン（例：`mycompany.cybozu.com`。サブドメイン名のみではなく、`.cybozu.com` 等を含むホスト名全体を指定してください）
- `generates[*].appId`: 対象アプリID
- `generates[*].apiToken`: API トークン（参照権限が必要）
- `generates[*].csharp.namespace`（C#）: モデルクラスの名前空間
- `generates[*].fileWriter.outputDirectory`: コード生成先

### 3. コード生成

```bash
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --lang CSharp --out-dir ./Models
```

または、設定ファイルを使用（`kmodel` を引数なしで実行すると、実行ファイルと同じディレクトリの `kmodel.config.json` を読み込みます）：

```bash
kmodel
```

### 4. 出力の確認

指定した `outputDirectory` に生成されたモデルクラスが配置されます。

---

## Configuration File (kmodel.config.json)

`kmodel.config.json` は、複数アプリの生成・エクスポート・diff 設定を一元管理するメインの設定ファイルです。

### 構造

```jsonc
{
  // JSON Schema による補完を有効化
  "$schema": "./kmodel.config.schema.json",

  // 必須: Kintone の完全なドメイン（サブドメイン名のみではなく .cybozu.com 等を含むホスト名全体）
  "subDomain": "mycompany.cybozu.com",

  // 生成設定の配列
  "generates": [
    {
      // 必須: Kintone アプリID
      "appId": 111,

      // 必須: このアプリへのアクセストークン
      "apiToken": "YOUR_API_TOKEN_HERE",

      // 任意: 生成言語（デフォルト: CSharp）
      // 選択肢: CSharp, Python, TypeScript, Go, Java
      "generateLanguage": "CSharp",

      // 任意: 選択肢を enum として生成するか（デフォルト: false）
      "generateEnums": false,

      // 任意: クラス名を固定（null で自動生成）
      "className": null,

      // C# 固有オプション
      "csharp": {
        "namespace": "KintoneModels.App111",
        "useRecord": false,
        "usePartial": true,
        "nullableEnabled": true,
        "pureMode": false
      },

      // Python 固有オプション（generateLanguage が Python の場合）
      "python": {
        "moduleName": "kintone_models",
        "useTypeHint": true
      },

      // ファイル出力オプション
      "fileWriter": {
        "outputDirectory": "./Models/App111",
        "overwriteExistingFiles": false
      }
    }
  ],

  // 任意: Raw Metadata エクスポート設定
  "raws": [
    {
      "appId": 111,
      "apiToken": "YOUR_API_TOKEN_HERE",
      "outputDirectory": "./Metadata",
      "escapeUnicode": false,
      "overwriteExistingFiles": false
    }
  ],

  // 任意: Metadata Diff 設定
  "diffs": [
    {
      "appId": 111,
      "apiToken": "YOUR_API_TOKEN_HERE",
      "outputDirectory": "./Diffs",
      "outputFormat": "Text",
      "diffOutputFile": "App111_Diff.txt",
      "compareWithFile": null,
      "failIfNoBaseFile": false
    }
  ]
}
```

### 主要オプション解説

#### 共通オプション（`generates[*]` 直下）

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `appId` | int | 必須 | Kintone アプリID |
| `apiToken` | string | 必須 | API トークン |
| `generateLanguage` | string | `CSharp` | 生成言語 |
| `generateEnums` | bool | false | 選択肢を enum 化 |
| `splitSubTableFiles` | bool | true | サブテーブルを別ファイルに分割 |
| `splitHelperFiles` | bool | true | 補助クラスを別ファイルに分割 |
| `className` | string? | null | クラス名（null で自動生成） |
| `headerComment` | string? | null | 生成ファイルの先頭コメント |

#### C# 固有オプション（`generates[*].csharp`）

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `namespace` | string | `KintoneModels` | 生成クラスの名前空間 |
| `useRecord` | bool | false | `record` 型を使用（.NET 5.0+） |
| `usePartial` | bool | true | `partial class` で生成 |
| `nullableEnabled` | bool | true | `#nullable enable` を付与 |
| `pureMode` | bool | false | KintoneNetLibrary 属性を除外 |

#### Python 固有オプション（`generates[*].python`）

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `moduleName` | string | `kintone_models` | Python モジュール名 |
| `useTypeHint` | bool | true | Type Hint を付与 |

#### ファイル出力オプション（`generates[*].fileWriter`）

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `outputDirectory` | string | `.` | 出力先ディレクトリ |
| `overwriteExistingFiles` | bool | false | ファイル上書きの可否 |

### VS Code での自動補完

`kmodel.config.schema.json` をプロジェクトに含めることで、VS Code で自動補完が有効になります。

```json
{
  "$schema": "./kmodel.config.schema.json",
  ...
}
```

---

## CLI Commands

### generate

Kintone のメタデータからモデルコードを生成します。

```bash
kmodel generate --sub-domain <SUBDOMAIN> --app-id <APP_ID> --api-token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--sub-domain` | string | ○（`--fields` 未指定時） | Kintone の完全なドメイン（例：`mycompany.cybozu.com`） |
| `--app-id` | int | ○ | Kintone アプリID |
| `--api-token` | string | ○（`--fields` 未指定時） | API トークン |
| `--lang` | string | × | 生成言語（デフォルト: CSharp） |
| `--out-dir` | string | × | 出力ディレクトリ |
| `--fields` | string | × | kbackup 由来の `fields.json` のパス（指定時は同じディレクトリの `layout.json` も読み込み、Kintone への接続なしで生成） |
| `--name-table` | string | × | プロパティ名変換テーブル（JSON）のパス |
| `--class-name` | string | × | 生成クラス名を固定したい場合に指定 |
| `--export-name-table-template` | string | × | 名前テーブルのテンプレートを出力して終了（モデルコードは生成しない） |

#### 使用例

```bash
# C# コードを生成
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --lang CSharp --out-dir ./Models

# Python コードを生成
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --lang Python --out-dir ./models

# TypeScript コードを生成
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --lang TypeScript --out-dir ./models
```

### raw

Kintone API から取得した生のメタデータを JSON ファイルにエクスポートします。

```bash
kmodel raw --sub-domain <SUBDOMAIN> --app-id <APP_ID> --api-token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--sub-domain` | string | ○（`--fields` 未指定時） | Kintone の完全なドメイン（例：`mycompany.cybozu.com`） |
| `--app-id` | int | ○ | Kintone アプリID |
| `--api-token` | string | ○（`--fields` 未指定時） | API トークン |
| `--out-dir` | string | × | 出力ディレクトリ |
| `--escape` | flag | × | 出力内のフィールドコードをエスケープする |
| `--pretty` | flag | × | 出力 JSON を整形する |

#### 使用例

```bash
# メタデータを JSON で保存
kmodel raw --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --out-dir ./metadata
```

### diff

2つの時点のメタデータを比較し、変更内容を表示します。

```bash
kmodel diff --sub-domain <SUBDOMAIN> --app-id <APP_ID> --api-token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--sub-domain` | string | ○（`--fields` 未指定時） | Kintone の完全なドメイン（例：`mycompany.cybozu.com`） |
| `--app-id` | int | ○ | Kintone アプリID |
| `--api-token` | string | ○（`--fields` 未指定時） | API トークン |
| `--out-dir` | string | × | 出力ディレクトリ |
| `--format` | string | × | 出力形式（Text / Json / Markdown、デフォルト: Text） |
| `--compare-with` | string | × | 比較対象の過去メタデータ JSON ファイルパス |
| `--escape` | flag | × | 出力内のフィールドコードをエスケープする |
| `--pretty` | flag | × | 出力 JSON を整形する |

#### 使用例

```bash
# 差分をテキスト形式で出力（デフォルト）
kmodel diff --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --out-dir ./raw

# Markdown 形式で出力
kmodel diff --sub-domain mycompany.cybozu.com --app-id 123 --api-token abc123 --format Markdown --out-dir ./diffs
```

### init

初期設定ファイルとスキーマを作成します。

```bash
kmodel init --lang <LANGUAGE>
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--lang` | string | ○ | コード生成対象言語 |

#### 使用例

```bash
# C# 用の初期設定を作成
kmodel init --lang CSharp

# Python 用の初期設定を作成
kmodel init --lang Python

# 出力:
#   - kmodel.config.json
```

---

## Examples

### 例1: 単一アプリの C# コード生成

```bash
# 1. 初期化（kmodel.config.json が作成される）
kmodel init --lang CSharp
# kmodel.config.json を編集

# 2. 生成実行（CLI オプションを直接指定する場合）
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --lang CSharp --out-dir ./Models
```

**生成されるファイル例（C#）**:

```
Models/
├── YourAppName.cs          # メインモデルクラス
├── SubtableRecord.cs       # サブテーブル用クラス
└── Enums/
    ├── StatusField.cs      # ドロップダウン enum
    └── CategoryField.cs
```

### 例2: 複数アプリを一度に処理

`kmodel.config.json`:

```json
{
  "$schema": "./kmodel.config.schema.json",
  "subDomain": "mycompany.cybozu.com",
  "generates": [
    {
      "appId": 111,
      "apiToken": "token111",
      "csharp": { "namespace": "KintoneModels.Customers" },
      "fileWriter": { "outputDirectory": "./Models/Customers" }
    },
    {
      "appId": 222,
      "apiToken": "token222",
      "csharp": { "namespace": "KintoneModels.Orders" },
      "fileWriter": { "outputDirectory": "./Models/Orders" }
    },
    {
      "appId": 333,
      "apiToken": "token333",
      "csharp": { "namespace": "KintoneModels.Products" },
      "fileWriter": { "outputDirectory": "./Models/Products" }
    }
  ]
}
```

実行（`kmodel.config.json` と同じディレクトリで、引数なしで `kmodel` を実行）:

```bash
kmodel
```

### 例3: Python コード生成

```bash
kmodel init --lang Python
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --lang Python --out-dir ./models
```

**生成されるファイル例（Python）**:

```
models/
├── your_app_name.py
├── __init__.py
└── enums.py
```

### 例4: Metadata Diff の取得

```bash
# 現在のメタデータを保存
kmodel raw --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --out-dir ./metadata

# 時間が経過後、差分を確認
kmodel diff --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --out-dir ./diffs --format Markdown
```

### 例5: TypeScript / JavaScript での使用

```bash
kmodel generate --sub-domain mycompany.cybozu.com --app-id 123 --api-token YOUR_TOKEN --lang TypeScript --out-dir ./models
```

生成ファイルを React / Vue.js プロジェクトで型定義として使用できます。

---

## Project Structure

```
kmodel/
├── Commands/                          # CLI コマンド定義
│   ├── RootCommandBuilder.cs         # ルートコマンド
│   ├── BaseCommandBuilder.cs         # コマンド基底クラス
│   ├── GenerateCommandBuilder.cs     # generate コマンド
│   ├── RawCommandBuilder.cs          # raw コマンド
│   ├── DiffCommandBuilder.cs         # diff コマンド
│   ├── InitCommandBuilder.cs         # init コマンド
│   └── OptionFactory.cs              # CLI オプション定義のファクトリー
│
├── Config/                            # 設定ファイル管理
│   ├── ConfigLoader.cs               # JSON 設定の読み込み
│   ├── KModelConfig.cs               # 設定ルートモデル
│   ├── GenerateConfig.cs             # generates エントリモデル
│   ├── RawConfig.cs                  # raws エントリモデル
│   └── DiffConfig.cs                 # diffs エントリモデル
│
├── Options/                           # 実行時オプション
│   ├── GenerateOptions.cs            # generate オプション
│   ├── CSharpOptions.cs              # C# 固有オプション
│   ├── PythonOptions.cs              # Python 固有オプション
│   ├── FileWriterOptions.cs          # ファイル出力オプション
│   ├── RawOptions.cs                 # raw オプション
│   └── DiffOptions.cs                # diff オプション
│
├── Interfaces/                        # 拡張ポイント
│   ├── IEmitterOptionsConverter.cs   # Options → EmitterOptions 変換
│   ├── IFileWriter.cs                # ファイル出力インターフェース
│   └── ILocalSchemaProviderFactory.cs # ローカルスキーマプロバイダーファクトリー
│
├── Services/                          # コア処理
│   ├── KModelApplication.cs          # アプリケーション本体
│   ├── RunnerBase.cs                 # Runner 基底クラス
│   ├── GenerateRunner.cs             # コード生成実行
│   ├── RawRunner.cs                  # Raw メタデータ出力実行
│   ├── DiffRunner.cs                 # Diff 実行
│   ├── EmitterOptionsConverter.cs    # GenerateOptions → EmitterOptions 変換
│   ├── FileWriter.cs                 # ファイル出力
│   └── LocalSchemaProviderFactory.cs # ローカルスキーマプロバイダーファクトリー実装
│
├── Extensions/                        # 拡張メソッド
│   └── KintoneMetadataDiffExtensions.cs # Diff 結果のテキスト/Markdown 変換
│
├── Enums/                             # 列挙型定義
│   └── DiffOutputFormats.cs          # Diff 出力形式（Text / Json / Markdown）
│
├── Logging/                           # ログ定義
│   └── StaticLogMessages.cs          # 構造化ログメッセージ定義
│
├── Properties/
│   └── AssemblyInfo.cs               # アセンブリ属性（InternalsVisibleTo 等）
│
├── Program.cs                         # エントリーポイント
├── kmodel.csproj
├── kmodel.config.json                 # 設定ファイル（実運用用）
├── kmodel.config.json.sample          # 設定ファイルサンプル
└── kmodel.config.schema.json          # JSON Schema

kmodel.Tests/                          # ユニットテスト（xUnit）
├── Config/
│   ├── ConfigLoaderTests.cs          # ConfigLoader.Load のデシリアライズ
│   └── ConfigToOptionsTests.cs       # *Config.ToOptions() マッピング
├── Extensions/
│   └── KintoneMetadataDiffExtensionsTests.cs # ToText / ToMarkdown
└── Services/
    ├── EmitterOptionsConverterTests.cs # ConvertToEmitterOptions 変換ロジック
    ├── FileWriterTests.cs             # 上書き制御・ファイル名検証
    └── GenerateRunnerCsvTests.cs      # CSV/TSV パーサー（ParseDelimitedLine / Escape）
```

---

## Extending kmodel

### 新しい言語対応を追加する方法

kmodel は、`KintoneNetLibrary.CodeGen` の **Emitter** パターンを活用して複数言語に対応しています。新言語を追加する場合、以下の手順に従います。

#### 1. KintoneNetLibrary.CodeGen に Emitter クラスを実装

`ICodeEmitter` インターフェースを実装した言語別 Emitter を作成します。

```csharp
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.CodeGen.Domain.Schemas;

namespace KintoneNetLibrary.CodeGen.Emitters;

public class RustCodeEmitter : ICodeEmitter {
    public CodeEmitResult Emit(
        KintoneAppSchema schema,
        CodeEmitterOptions options,
        INameConverter nameConverter) {
        // Rust コード生成ロジック
        var code = GenerateRustCode(schema, options, nameConverter);
        return new CodeEmitResult { MainModelCode = code };
    }
}
```

#### 2. 言語列挙型に追加

`KintoneNetLibrary.CodeGen.Domain.Enums.GenerateLanguages`:

```csharp
public enum GenerateLanguages {
    CSharp,
    Python,
    TypeScript,
    Go,
    Java,
    Rust  // 追加
}
```

#### 3. kmodel 側の EmitterOptionsConverter に分岐を追加

`Services/EmitterOptionsConverter.cs` の `ConvertToEmitterOptions` メソッドに新言語の `case` を追加します。

```csharp
GenerateLanguages.Rust => new RustEmitterOptions {
    // 共通プロパティのマッピング
    MainClassName = options.ClassName ?? ...,
    ...
},
```

---

## Contributing

kmodel への貢献を歓迎します！

### 貢献の流れ

1. **Fork** してフィーチャーブランチを作成
   ```bash
   git checkout -b feature/add-rust-support
   ```

2. **変更を追加してコミット**
   ```bash
   git commit -m "feat: Add Rust language support"
   ```

3. **ブランチをプッシュ**
   ```bash
   git push origin feature/add-rust-support
   ```

4. **Pull Request を作成**

### コード規約

- **言語**: C# 10.0 以上
- **フレームワーク**: .NET 10.0
- **コメント**: 日本語
- **スタイル**: `this.` を使用、`{}` を省略しない
- **テスト**: xUnit でユニットテストを記述（`dotnet test` で実行）
- **ドキュメント**: XML コメント（`///`）を使用

### 開発環境のセットアップ

```bash
# クローン
git clone https://github.com/your-organization/kmodel.git
cd kmodel

# ビルド
dotnet build kmodel.slnx

# テスト実行
dotnet test

# リリース版ビルド
dotnet build -c Release
```

---

## License

kmodel は [MIT License](LICENSE) の下で公開されています。
これは依存ライブラリである KintoneNetLibrary と同じライセンスです。

---

## Support

問題が発生した場合は、[GitHub Issues](https://github.com/your-organization/kmodel/issues) で報告してください。
