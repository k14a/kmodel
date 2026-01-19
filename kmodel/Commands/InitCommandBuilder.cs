using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using kmodel.Config;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Domain.Enums;

namespace kmodel.Commands;

public static class InitCommandBuilder {
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

    private static KModelConfig CreateTemplate(string lang) {
        var config = new KModelConfig {
            SubDomain = "example",
            Generates = [],
            Raws = [],
            Diffs = []
        };

        var generate = new GenerateOptions {
            AppId = 0,
            ApiToken = "",
            GenerateLanguage = Enum.Parse<GenerateLanguages>(lang, true),
            OutputDirectory = "./Models"
        };

        // -------------------------
        // 言語別の追加項目
        // -------------------------
        switch (generate.GenerateLanguage) {
            case GenerateLanguages.CSharp:
                generate.Namespace = "KintoneModels";
                generate.UseRecord = false;
                generate.UsePartial = true;
                generate.NullableEnabled = true;
                break;

            case GenerateLanguages.Python:
                generate.ModuleName = "kintone_models";
                generate.UseTypeHint = true;
                break;

            case GenerateLanguages.TypeScript:
                generate.ModuleName = "kintone-models";
                break;

            case GenerateLanguages.Go:
                generate.ModuleName = "kintone_models";
                break;

            case GenerateLanguages.Java:
                generate.Namespace = "kintone.models";
                break;
        }

        config.Generates.Add(generate);
        return config;
    }
}