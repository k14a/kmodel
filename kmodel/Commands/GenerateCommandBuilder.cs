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

        var sub = OptionFactory.SubDomain();
        var app = OptionFactory.AppId();
        var token = OptionFactory.ApiToken();
        var lang = OptionFactory.Language();
        var outDir = OptionFactory.OutputDirectory("Output directory for generated code");

        this.AddOptions(cmd, sub, app, token, lang, outDir);

        this.AddExamples(cmd,
            "kmodel generate --sub-domain example --app-id 123 --api-token ABCDEF --lang CSharp --out-dir ./Models",
            "kmodel generate --sub-domain example --app-id 456 --api-token XYZ --lang TypeScript --out-dir ./ts"
        );

        this.SetHandler(cmd, async parse => {
            var g = new GenerateOptions {
                AppId = parse.GetValue(app),
                ApiToken = parse.GetValue(token),
                GenerateLanguage = Enum.Parse<GenerateLanguages>(parse.GetValue(lang), true),
                OutputDirectory = parse.GetValue(outDir) ?? "."
            };

            var runner = host.Services.GetRequiredService<GenerateRunner>();
            await runner.RunAsync(g, parse.GetValue(sub));
        });

        return cmd;
    }
}
