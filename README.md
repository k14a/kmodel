# kmodel

## 📘 設定ファイル（kmodel.config.json）について

kmodel は、**kintone アプリのフィールド情報から C# モデルコードを生成するツール**です。  
動作設定は `kmodel.config.json` に記述します。

初めて利用する場合は、リポジトリに含まれる  
**`kmodel.config.json.sample` を `kmodel.config.json` にリネームして編集**してください。

---

### 📌 `$schema` による補完サポート

VS Code を利用している場合、設定ファイルの先頭に `$schema` を指定すると、  
**自動補完・バリデーションが有効**になります。

```jsonc
{
  "$schema": "./kmodel.config.schema.json",
  "generate": [ ... ]
}
```

---

## 🧩 設定ファイルの構造

`kmodel.config.json` は次のような構造を持ちます。

```jsonc
{
  "generate": [
    {
      "appId": 123,
      "apiToken": "YOUR_API_TOKEN",
      "namespace": "KintoneModels.App123",
      "className": null,
      "useRecord": false,
      "pureMode": false,
      "generateEnums": true,
      "output": "./Models/App123"
    }
  ]
}
```

---

## 📝 各項目の説明

### 🔹 generate（配列）
kmodel が生成処理を行う対象アプリの設定を **複数件**記述できます。

---

### 🔸 appId（必須）
kintone アプリの ID を指定します。

例：  
```json
"appId": 123
```

---

### 🔸 apiToken（必須）
対象アプリのフィールド情報を取得するための API トークン。

例：  
```json
"apiToken": "YOUR_API_TOKEN"
```

---

### 🔸 namespace（任意）
生成される C# クラスの namespace。

省略時は `"KintoneModels"` が使用されます。

例：  
```json
"namespace": "KintoneModels.App123"
```

---

### 🔸 className（任意）
生成するクラス名を固定したい場合に指定します。  
`null` の場合はアプリ名などから自動生成されます。

例：  
```json
"className": "Customer"
```

---

### 🔸 useRecord（任意）
C# の `record` を使用してモデルを生成するかどうか。

```json
"useRecord": true
```

---

### 🔸 pureMode（任意）
kintone 固有の属性（`[KintoneField]` など）を付与しない  
**純粋な POCO モデル**を生成します。

```json
"pureMode": false
```

---

### 🔸 generateEnums（任意）
ドロップダウンや複数選択フィールドを enum として生成するかどうか。

```json
"generateEnums": true
```

---

### 🔸 output（任意）
生成されたファイルの出力先ディレクトリ。

```json
"output": "./Models/App123"
```

---

## 📦 複数アプリをまとめて生成する例

```jsonc
{
  "generate": [
    {
      "appId": 123,
      "apiToken": "TOKEN_123",
      "namespace": "KintoneModels.App123",
      "output": "./Models/App123"
    },
    {
      "appId": 456,
      "apiToken": "TOKEN_456",
      "namespace": "KintoneModels.App456",
      "className": "Customer",
      "useRecord": true,
      "generateEnums": false,
      "output": "./Models/App456"
    }
  ]
}
```

---
