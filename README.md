# kmodel

Kintone の metadata.json / fields.json をシームレスに扱う CLI ツール。複数の言語（C#、Python、TypeScript、Go、Java）へのコード生成、raw metadata のエクスポート、メタデータの差分検出を支援します。

---

## 目次

1. [Introduction](#introduction)
2. [Features](#features)
3. [Installation](#installation)
4. [Quick Start](#quick-start)
5. [Configuration File](#configuration-file-kmodelconfigjson)
6. [CLI Commands](#cli-commands)
7. [Examples](#examples)
8. [Project Structure](#project-structure)
9. [Extending kmodel](#extending-kmodel)
10. [Contributing](#contributing)
11. [License](#license)

---

## Introduction

**kmodel** は、Kintone の API から取得したメタデータを活用し、以下の処理を自動化する CLI ツールです：

- 🔧 **複数言語へのコード自動生成**: Kintone のアプリスキーマから、C#、Python、TypeScript、Go、Java のモデルコードを生成
- 📋 **Raw Metadata Export**: Kintone API から取得したメタデータを JSON 形式で保存
- 🔍 **Metadata Diff**: 異なるバージョンのメタデータを比較し、変更内容を可視化
- ⚙️ **一括処理**: `kmodel.config.json` で複数アプリの設定を一元管理
- 🖥️ **System.CommandLine による直感的 CLI**: .NET の最新 CLI フレームワークで統一された使用感

kmodel は内部で Kintone API との通信およびメタデータ処理に  
[KintoneNetLibrary](https://github.com/k14a/KintoneNetLibrary.git) を使用しています。

**対応言語**:
- C# (.NET)
- Python
- TypeScript
- Go
- Java

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

このコマンドは、カレントディレクトリに `kmodel.config.json.sample` と `kmodel.config.schema.json` を作成します。

```bash
cp kmodel.config.json.sample kmodel.config.json
```

### 2. 設定ファイルを編集

エディタで `kmodel.config.json` を開き、以下を設定：

- `subDomain`: あなたの Kintone サブドメイン（例：`mycompany`）
- `appId`: 対象アプリID
- `apiToken`: API トークン（参照権限が必要）
- `namespace`（C#）: モデルクラスの名前空間
- `outputDirectory`: コード生成先

### 3. コード生成

```bash
kmodel generate --subdomain mycompany --app 123 --token YOUR_TOKEN --lang CSharp --out ./Models
```

または、設定ファイルを使用：

```bash
kmodel generate --config kmodel.config.json
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

  // 必須: Kintone サブドメイン
  "subDomain": "mycompany",

  // 生成設定の配列
  "generates": [
    {
      // 必須: Kintone アプリID
      "appId": 111,

      // 必須: このアプリへのアクセストークン
      "apiToken": "YOUR_API_TOKEN_HERE",

      // 任意: C# クラスの名前空間（デフォルト: KintoneModels）
      "namespace": "KintoneModels.App111",

      // 任意: クラス名を固定（null で自動生成）
      "className": null,

      // 任意: record 型を使用するか（デフォルト: false）
      "useRecord": false,

      // 任意: Pure モード - フレームワーク依存なしの POCO を生成（C# 専用）
      "pureMode": false,

      // 任意: 選択肢を enum として生成するか（デフォルト: false）
      "generateEnums": false,

      // 任意: 生成言語（デフォルト: CSharp）
      // 選択肢: CSharp, Python, TypeScript, Go, Java
      "generateLanguage": "CSharp",

      // 任意: 出力ディレクトリ（デフォルト: .）
      "outputDirectory": "./Models/App111",

      // 任意: 既存ファイルを上書きするか（デフォルト: false）
      "overwriteExistingFiles": false
    }
  ],

  // 任意: Raw Metadata エクスポート設定
  "raws": [
    {
      "appId": 111,
      "apiToken": "YOUR_API_TOKEN_HERE",
      "outputDirectory": "./Metadata",
      "overwriteExistingFiles": false
    }
  ],

  // 任意: Metadata Diff 設定
  "diffs": [
    {
      "appId": 111,
      "apiToken": "YOUR_API_TOKEN_HERE",
      "outputDirectory": "./Diffs",
      "outputFormat": "json"
    }
  ]
}
```

### 主要オプション解説

#### C# 固有オプション

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `namespace` | string | `KintoneModels` | 生成クラスの名前空間 |
| `className` | string? | null | クラス名（null で自動生成） |
| `useRecord` | bool | false | `record` 型を使用（.NET 5.0+） |
| `usePartial` | bool | true | `partial class` で生成 |
| `nullableEnabled` | bool | true | `#nullable enable` を付与 |
| `pureMode` | bool | false | KintoneNetLibrary 属性を除外 |

#### Python 固有オプション

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `moduleName` | string | `kintone_models` | Python モジュール名 |
| `useTypeHint` | bool | true | Type Hint を付与 |

#### 共通オプション

| オプション | 型 | デフォルト | 説明 |
|---|---|---|---|
| `appId` | int | 必須 | Kintone アプリID |
| `apiToken` | string | 必須 | API トークン |
| `generateEnums` | bool | false | 選択肢を enum 化 |
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
kmodel generate --subdomain <SUBDOMAIN> --app <APP_ID> --token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--subdomain` | string | ○ | Kintone サブドメイン |
| `--app` | int | ○ | Kintone アプリID |
| `--token` | string | ○ | API トークン |
| `--lang` | string | × | 生成言語（デフォルト: CSharp） |
| `--out` | string | × | 出力ディレクトリ（デフォルト: .） |
| `--config` | string | × | 設定ファイルパス |

#### 使用例

```bash
# C# コードを生成
kmodel generate --subdomain mycompany --app 123 --token abc123 --lang CSharp --out ./Models

# Python コードを生成
kmodel generate --subdomain mycompany --app 123 --token abc123 --lang Python --out ./models

# TypeScript コードを生成
kmodel generate --subdomain mycompany --app 123 --token abc123 --lang TypeScript --out ./models
```

### raw

Kintone API から取得した生のメタデータを JSON ファイルにエクスポートします。

```bash
kmodel raw --subdomain <SUBDOMAIN> --app <APP_ID> --token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--subdomain` | string | ○ | Kintone サブドメイン |
| `--app` | int | ○ | Kintone アプリID |
| `--token` | string | ○ | API トークン |
| `--out` | string | × | 出力ディレクトリ（デフォルト: .） |

#### 使用例

```bash
# メタデータを JSON で保存
kmodel raw --subdomain mycompany --app 123 --token abc123 --out ./metadata

# 出力: ./metadata/123_metadata.json
```

### diff

2つの時点のメタデータを比較し、変更内容を表示します。

```bash
kmodel diff --subdomain <SUBDOMAIN> --app <APP_ID> --token <TOKEN> [OPTIONS]
```

#### オプション

| オプション | 型 | 必須 | 説明 |
|---|---|---|---|
| `--subdomain` | string | ○ | Kintone サブドメイン |
| `--app` | int | ○ | Kintone アプリID |
| `--token` | string | ○ | API トークン |
| `--out` | string | × | 出力ディレクトリ（デフォルト: .） |
| `--format` | string | × | 出力形式（json / yaml / markdown） |

#### 使用例

```bash
# 差分をコンソールに出力（デフォルト）
kmodel diff --subdomain mycompany --app 123 --token abc123

# JSON 形式で出力
kmodel diff --subdomain mycompany --app 123 --token abc123 --format json --out ./diffs
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
#   - kmodel.config.json.sample
#   - kmodel.config.schema.json
```

---

## Examples

### 例1: 単一アプリの C# コード生成

```bash
# 1. 初期化
kmodel init --lang CSharp

# 2. 設定ファイルを作成・編集
cp kmodel.config.json.sample kmodel.config.json
# kmodel.config.json を編集

# 3. 生成実行
kmodel generate --subdomain mycompany --app 123 --token YOUR_TOKEN --lang CSharp --out ./Models
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
  "subDomain": "mycompany",
  "generates": [
    {
      "appId": 111,
      "apiToken": "token111",
      "namespace": "KintoneModels.Customers",
      "outputDirectory": "./Models/Customers"
    },
    {
      "appId": 222,
      "apiToken": "token222",
      "namespace": "KintoneModels.Orders",
      "outputDirectory": "./Models/Orders"
    },
    {
      "appId": 333,
      "apiToken": "token333",
      "namespace": "KintoneModels.Products",
      "outputDirectory": "./Models/Products"
    }
  ]
}
```

実行:

```bash
kmodel generate --config kmodel.config.json
```

### 例3: Python コード生成

```bash
kmodel init --lang Python
kmodel generate --subdomain mycompany --app 123 --token YOUR_TOKEN --lang Python --out ./models
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
kmodel raw --subdomain mycompany --app 123 --token YOUR_TOKEN --out ./metadata

# 時間が経過後、差分を確認
kmodel diff --subdomain mycompany --app 123 --token YOUR_TOKEN --out ./diffs --format markdown
```

### 例5: TypeScript / JavaScript での使用

```bash
kmodel generate --subdomain mycompany --app 123 --token YOUR_TOKEN --lang TypeScript --out ./models
```

生成ファイルを React / Vue.js プロジェクトで型定義として使用できます。

---

## Project Structure

```
kmodel/
├── Commands/                     # CLI コマンド定義
│   ├── RootCommandBuilder.cs    # ルートコマンド
│   ├── GenerateCommandBuilder.cs # generate コマンド
│   ├── RawCommandBuilder.cs      # raw コマンド
│   ├── DiffCommandBuilder.cs     # diff コマンド
│   └── InitCommandBuilder.cs     # init コマンド
│
├── Options/                      # CLI オプション定義
│   ├── GenerateOptions.cs       # generate オプション
│   ├── RawOptions.cs            # raw オプション
│   ├── DiffOptions.cs           # diff オプション
│   ├── FileWriterOptions.cs     # ファイル出力オプション
│   └── GenerateOptionsExtensions.cs
│
├── Config/                       # 設定ファイル管理
│   ├── ConfigLoader.cs          # JSON 設定の読み込み
│   └── KModelConfig.cs          # 設定データモデル
│
├── Services/                     # コア処理
│   ├── CodeGenRunner.cs         # コード生成実行
│   ├── SchemaConverter.cs       # メタデータ → スキーマ変換
│   ├── FileWriter.cs            # ファイル出力
│   └── KModelApplication.cs     # アプリケーション本体
│
├── Interfaces/                   # 拡張ポイント
│   ├── ISchemaConverter.cs      # スキーマ変換インターフェース
│   └── IFileWriter.cs           # ファイル出力インターフェース
│
├── Enums/                        # 列挙型定義
│   └── DiffOutputFormats.cs     # Diff 出力形式
│
├── Extensions/                   # 拡張メソッド
│   └── KintoneMetadataDiffExtensions.cs
│
├── Program.cs                    # エントリーポイント
├── kmodel.csproj               # プロジェクトファイル
├── kmodel.config.json          # 設定ファイル（実運用用）
├── kmodel.config.json.sample   # 設定ファイルサンプル
└── kmodel.config.schema.json   # JSON Schema
```

---

## Extending kmodel

### 新しい言語対応を追加する方法

kmodel は、`KintoneNetLibrary.CodeGen` の **Emitter** パターンを活用して複数言語に対応しています。新言語を追加する場合、以下の手順に従います。

#### 1. Emitter クラスの実装

`KintoneNetLibrary.CodeGen` プロジェクトに、言語ごとの `ICodeEmitter` 実装を作成：

```csharp
using KintoneNetLibrary.CodeGen.Domain.Models;

namespace KintoneNetLibrary.CodeGen.Emitters;

public class RustCodeEmitter : ICodeEmitter
{
    private readonly CodeGenContext _context;

    public RustCodeEmitter(CodeGenContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string>> EmitAsync()
    {
        var files = new Dictionary<string, string>();

        // Rust コード生成ロジック
        var rustCode = GenerateRustCode();
        files["model.rs"] = rustCode;

        return files;
    }

    private string GenerateRustCode()
    {
        // 実装
        return "";
    }
}
```

#### 2. 言語列挙型に追加

Enums/GenerateLanguages.cs:

```csharp
public enum GenerateLanguages
{
    CSharp,
    Python,
    TypeScript,
    Go,
    Java,
    Rust  // 追加
}
```

#### 3. ファクトリーで登録

`CodeGenRunner` の `GetEmitter` メソッドを更新：

```csharp
private ICodeEmitter GetEmitter(GenerateLanguages language, CodeGenContext context)
{
    return language switch
    {
        GenerateLanguages.CSharp => new CSharpCodeEmitter(context),
        GenerateLanguages.Python => new PythonCodeEmitter(context),
        GenerateLanguages.Rust => new RustCodeEmitter(context), // 追加
        _ => throw new NotSupportedException($"Language {language} is not supported")
    };
}
```

#### 4. テストを追加

```csharp
[Fact]
public async Task EmitAsync_GeneratesValidRustCode()
{
    var context = new CodeGenContext { /* ... */ };
    var emitter = new RustCodeEmitter(context);
    var result = await emitter.EmitAsync();

    Assert.NotEmpty(result);
    Assert.Contains("model.rs", result.Keys);
}
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
- **スタイル**: Microsoft の [C# コーディング規約](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)に準拠
- **テスト**: xUnit で単体テストを記述
- **ドキュメント**: XML コメント（`///`）を使用

### 開発環境のセットアップ

```bash
# クローン
git clone https://github.com/your-organization/kmodel.git
cd kmodel

# ビルド
dotnet build

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

---

**Made with ❤️ by the Kintone Community**
