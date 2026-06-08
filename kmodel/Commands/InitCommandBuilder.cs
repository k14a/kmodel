using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using kmodel.Config;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Domain.Enums;

namespace kmodel.Commands;

/// <summary>
/// Init コマンドのビルダークラス。ユーザーが "kmodel init" コマンドを実行したときに、テンプレートの kmodel.config.json ファイルを生成するためのコマンドを構築します。ユーザーは --lang オプションでコード生成の対象言語を指定でき、指定された言語に応じたテンプレートが生成されます。例えば、"kmodel init --lang TypeScript" と入力すると、TypeScript 向けのテンプレートが生成されます。
/// </summary>
public static class InitCommandBuilder {
    /// <summary>
    /// Init コマンドを構築する静的メソッド。ユーザーが "kmodel init" コマンドを実行したときに、テンプレートの kmodel.config.json ファイルを生成するためのコマンドを構築します。ユーザーは --lang オプションでコード生成の対象言語を指定でき、指定された言語に応じたテンプレートが生成されます。例えば、"kmodel init --lang TypeScript" と入力すると、TypeScript 向けのテンプレートが生成されます。
    /// </summary>
    /// <returns>構築された Command インスタンス</returns>
    public static Command Build() {
        var cmd = new Command("init", "Generate a template kmodel.config.json");

        var lang = new Option<string>("--lang") {
            Description = "Programming language for code generation (CSharp, Python, TypeScript, Go, Java)",
            DefaultValueFactory = _ => "CSharp"
        };
        cmd.Add(lang);

        cmd.Description += @"

### Examples

```bash
kmodel init
kmodel init --lang TypeScript
```";

        cmd.SetAction(async result => {
            var selectedLang = result.GetValue(lang) ?? "CSharp";
            var configPath = "kmodel.config.json";

            // -------------------------
            // 上書き確認
            // -------------------------
            if (File.Exists(configPath)) {
                Console.Write($"File '{configPath}' already exists. Overwrite? (y/N): ");
                var key = Console.ReadLine()?.Trim().ToLower();

                if (key != "y") {
                    Console.WriteLine("Canceled. No changes were made.");
                    return;
                }
            }

            // -------------------------
            // 言語別テンプレ生成
            // -------------------------
            var config = CreateTemplate(selectedLang);

            // -------------------------
            // JSON 書き込み
            // -------------------------
            var json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(configPath, json);

            Console.WriteLine($"✔ Created {configPath}{Environment.NewLine}Next step: edit the file and run `kmodel`");
        });

        return cmd;
    }

    /// <summary>
    /// 指定された言語に応じたテンプレートの KModelConfig インスタンスを作成する静的メソッド。ユーザーが --lang オプションで指定した言語に基づいて、KModelConfig の GenerateOptions に適切なプロパティを設定したテンプレートを生成します。例えば、"CSharp" を指定した場合は、C# 向けのプロパティが設定された GenerateOptions が含まれる KModelConfig が生成されます。
    /// </summary>
    /// <param name="lang">コード生成の対象言語</param>
    /// <returns>構築された KModelConfig インスタンス</returns>
    private static KModelConfig CreateTemplate(string lang) {
        var config = new KModelConfig {
            SubDomain = "example",
            Generates = [],
            Raws = [],
            Diffs = []
        };

        var generate = new GenerateConfig {
            AppId = 0,
            ApiToken = "",
            GenerateLanguage = Enum.Parse<GenerateLanguages>(lang, true),
            FileWriter = new FileWriterOptions { OutputDirectory = "./Models" }
        };

        // -------------------------
        // 言語別の追加項目
        // -------------------------
        switch (generate.GenerateLanguage) {
            case GenerateLanguages.CSharp:
                generate.CSharp.Namespace = "KintoneModels";
                generate.CSharp.UseRecord = false;
                generate.CSharp.UsePartial = true;
                generate.CSharp.NullableEnabled = true;
                break;

            case GenerateLanguages.Python:
                generate.Python.ModuleName = "kintone_models";
                generate.Python.UseTypeHint = true;
                break;

            // TypeScript / Go / Java は未実装のため共通オプションのみ設定
        }

        config.Generates.Add(generate);
        return config;
    }
}