# 🩺 kmodel doctor とは何か

**kmodel の動作環境・設定・接続性をまとめて診断するコマンド**。

ユーザーが「動かない…」と悩む前に、  
`kmodel doctor` を実行すれば原因がわかるようにする。

Homebrew の `brew doctor` や  
Node.js の `npm doctor` のような位置づけ。

---

# 🎯 kmodel doctor がチェックすべき項目（推奨）

## 1. kmodel.config.json の存在チェック
- ファイルが存在するか  
- JSON としてパースできるか  
- 必須項目（SubDomain, Generates, Raws, Diffs）が揃っているか  

## 2. SubDomain の疎通チェック
- `https://{subdomain}.cybozu.com/k/v1/app.json` に HEAD リクエスト  
- 200/401/403/404 の判定  
- DNS 解決できるか  

## 3. API Token の有効性チェック
- AppId + Token の組み合わせで  
  `GetMetadataAsync()` を実行してみる  
- 401 → Token 無効  
- 403 → 権限不足  
- 404 → AppId 不正  

## 4. OutputDirectory の書き込み権限チェック
- ディレクトリが存在するか  
- 作成できるか  
- 書き込み権限があるか  

## 5. GenerateOptions の妥当性チェック
- 言語名が正しいか  
- Namespace / ModuleName が空でないか  
- OutputDirectory が null の場合の挙動確認  

## 6. RawOptions / DiffOptions の妥当性チェック
- Raw の Pretty / EscapeUnicode の組み合わせ  
- Diff の CompareWithFile の存在チェック  

## 7. kmodel のバージョン表示
- CLI のバージョン  
- .NET ランタイムのバージョン  

---

# 🧱 doctor の出力イメージ（理想）

```
$ kmodel doctor

🔍 Running kmodel diagnostics...

✔ kmodel.config.json found
✔ JSON format valid
✔ SubDomain reachable: example.cybozu.com
✔ API Token valid for AppId 123
✔ Output directory writable: ./Models
✔ GenerateOptions valid (CSharp)
✔ RawOptions valid
✔ DiffOptions valid

🎉 All checks passed! kmodel is ready to use.
```

エラーがある場合：

```
❌ API Token invalid for AppId 123
   → Check your token or app permissions.

❌ Output directory not writable: ./Models
   → Permission denied.
```

---

# 🧩 doctor の設計（CommandBuilder + RunnerBase を活かす）

## doctor コマンドは Runner を使うべきか？
→ **使うべきではない**

理由：

- doctor は「生成」「出力」を行わない  
- RunnerBase の try/catch やログ構造は不要  
- doctor は「診断専用のサービス」として独立させるのが自然  

つまり：

```
DoctorService
DoctorCommandBuilder
```

という構造が最適。

---

# 🧱 DoctorService の構造案

```csharp
public class DoctorService
{
    private readonly ConfigLoader _configLoader;
    private readonly ISchemaProvider _schemaProvider;

    public DoctorService(ConfigLoader configLoader, ISchemaProvider schemaProvider)
    {
        _configLoader = configLoader;
        _schemaProvider = schemaProvider;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("🔍 Running kmodel diagnostics...\n");

        CheckConfigFile();
        await CheckSubDomainAsync();
        await CheckApiTokensAsync();
        CheckOutputDirectories();

        Console.WriteLine("\n🎉 All checks completed.");
    }
}
```

---

# 🧱 DoctorCommandBuilder の構造案

```csharp
public class DoctorCommandBuilder : BaseCommandBuilder
{
    public DoctorCommandBuilder(IHost host) : base(host) { }

    public Command Build()
    {
        var cmd = CreateCommand("doctor", "Diagnose kmodel environment and configuration");

        AddExamples(cmd,
            "kmodel doctor",
            "kmodel doctor --config ./kmodel.config.json"
        );

        SetHandler(cmd, async parse =>
        {
            var doctor = Host.Services.GetRequiredService<DoctorService>();
            await doctor.RunAsync();
        });

        return cmd;
    }
}
```

---

# 🌟 kmodel doctor の価値

### ✔ ユーザーの「動かない…」を一発で解決  
→ OSS のサポートコストが激減する。

### ✔ CLI のプロフェッショナル感が増す  
→ kmodel の信頼性が上がる。

### ✔ あなたの OSS 哲学に完全一致  
- UX  
- 安全性  
- 実用性  
- メンテナンス性  

---

