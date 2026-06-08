using System.CommandLine;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

/// <summary>
/// Generate コマンドのビルダークラス。ユーザーが "kmodel generate" コマンドを実行したときに、Kintone アプリのスキーマからモデルコードを生成するためのコマンドを構築します。ユーザーは --sub-domain、--app-id、--api-token、--lang、--out-dir の各オプションを指定して、生成するコードの対象となる Kintone アプリやコード生成の言語、出力ディレクトリなどを設定できます。例えば、"kmodel generate --sub-domain example --app-id 123 --api-token ABCDEF --lang CSharp --out-dir ./Models" と入力すると、example サブドメインの appId 123 のアプリから C# のモデルコードが生成され、./Models ディレクトリに保存されます。
/// </summary>
/// <param name="host">ホストインスタンス</param>
public class GenerateCommandBuilder(IHost host) : BaseCommandBuilder(host) {
    /// <summary>
    /// "generate" コマンドを構築するメソッド。ユーザーが "kmodel generate" コマンドを実行したときに、Kintone アプリのスキーマからモデルコードを生成するためのコマンドを構築します。ユーザーは --sub-domain、--app-id、--api-token、--lang、--out-dir の各オプションを指定して、生成するコードの対象となる Kintone アプリやコード生成の言語、出力ディレクトリなどを設定できます。例えば、"kmodel generate --sub-domain example --app-id 123 --api-token ABCDEF --lang CSharp --out-dir ./Models" と入力すると、example サブドメインの appId 123 のアプリから C# のモデルコードが生成され、./Models ディレクトリに保存されます。
    /// </summary>
    /// <returns>構築された Command インスタンス</returns>
    public Command Build() {
        var cmd = this.CreateCommand("generate", "Generate model code from Kintone app schema");

        var subDomainOption = OptionFactory.SubDomain();
        var appIdOption = OptionFactory.AppId();
        var apiTokenOption = OptionFactory.ApiToken();
        var languageOption = OptionFactory.Language();
        var outputDirectoryOption = OptionFactory.OutputDirectory("Output directory for generated code");
        var fieldsFileOption = OptionFactory.FieldsFile();
        var templateOption = OptionFactory.ExportNameTableTemplate();
        var nameTableOption = OptionFactory.NameTable();
        var classNameOption = OptionFactory.ClassName();

        this.AddOptions(
            cmd,
            subDomainOption,
            appIdOption,
            apiTokenOption,
            languageOption,
            outputDirectoryOption,
            fieldsFileOption,
            templateOption,
            nameTableOption,
            classNameOption);

        this.AddExamples(cmd,
            "kmodel generate --sub-domain example --app-id 123 --api-token ABCDEF --lang CSharp --out-dir ./Models",
            "kmodel generate --sub-domain example --app-id 456 --api-token XYZ --lang TypeScript --out-dir ./ts",
            "kmodel generate --fields ./fields.json --app-id 123 --lang CSharp --out-dir ./Models",
            $"# Export name table template only (no model code will be generated){Environment.NewLine}kmodel generate --sub-domain example --app-id 789 --api-token FOOBAR --export-name-table-template ./template.json",
            $"# Export name table template only (no model code will be generated){Environment.NewLine}kmodel generate --fields ./fields.json --app-id 456 --export-name-table-template ./template.json"
        );

        this.SetHandler(cmd, async parse => {
            var templateFile = parse.GetValue(templateOption);
            var fieldsFile = parse.GetValue(fieldsFileOption);
            var nameTableFile = parse.GetValue(nameTableOption);
            var subDomain = parse.GetValue(subDomainOption);
            var apiToken = parse.GetValue(apiTokenOption);
            var className = parse.GetValue(classNameOption);

            // --- 排他チェック ---
            if (!string.IsNullOrEmpty(fieldsFile)) {
                // fields.json モード
                if (subDomain is not null || apiToken is not null) {
                    throw new ArgumentException("--fields cannot be used with --sub-domain or --api-token");
                }
            } else {
                // API モード
                if (subDomain is null || apiToken is null) {
                    throw new ArgumentException("--sub-domain and --api-token are required unless --fields is specified");
                }
            }

            // --- GenerateOptions の構築 ---
            var g = new GenerateOptions {
                AppId = parse.GetValue(appIdOption),
                ApiToken = apiToken ?? string.Empty,
                FieldsFile = fieldsFile ?? string.Empty,
                GenerateLanguage = Enum.Parse<GenerateLanguages>(parse.GetValue(languageOption) ?? "CSharp", true),
                FileWriter = new Options.FileWriterOptions {
                    OutputDirectory = parse.GetValue(outputDirectoryOption) ?? ".",
                },
                ExportNameTableTemplate = templateFile ?? string.Empty,
                NameTable = nameTableFile ?? string.Empty,
                ClassName = className,
            };

            var runner = this._host.Services.GetRequiredService<GenerateRunner>();
            await runner.RunAsync(g, subDomain ?? string.Empty);
        });

        return cmd;
    }
}
