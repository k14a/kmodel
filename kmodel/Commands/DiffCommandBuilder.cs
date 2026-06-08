using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

/// <summary>
/// DiffCommandBuilder クラスは、kmodel CLI の "diff" コマンドを構築するためのクラスです。このコマンドは、Kintone アプリのメタデータをエクスポートし、以前のメタデータと比較して差分を出力する機能を提供します。ユーザーはサブドメイン、アプリID、APIトークン、出力ディレクトリ、Unicodeエスケープの有無、出力の整形オプション、差分出力形式、そして比較対象のファイルを指定して、このコマンドを実行できます。コマンドが実行されると、DiffRunner クラスが呼び出され、指定されたオプションに基づいて Kintone アプリのメタデータがエクスポートされ、以前のメタデータと比較して差分が出力されます。
/// </summary>
/// <param name="host">ホストインスタンス</param>
public class DiffCommandBuilder(IHost host) : BaseCommandBuilder(host) {
    /// <summary>
    /// "diff" コマンドを構築するメソッド。コマンドには、サブドメイン、アプリID、APIトークン、出力ディレクトリ、Unicodeエスケープの有無、出力の整形オプション、差分出力形式、そして比較対象のファイルが含まれます。ユーザーがコマンドを実行すると、DiffRunner クラスが呼び出され、指定されたオプションに基づいて Kintone アプリのメタデータがエクスポートされ、以前のメタデータと比較して差分が出力されます。
    /// </summary>
    /// <returns>構築された Command インスタンス</returns>
    public Command Build() {
        var cmd = this.CreateCommand("diff", "Compare metadata and output differences");

        var sub = OptionFactory.SubDomain();
        var app = OptionFactory.AppId();
        var token = OptionFactory.ApiToken();
        var outDir = OptionFactory.OutputDirectory("Output directory for metadata diff");
        var escape = OptionFactory.EscapeUnicode();
        var pretty = OptionFactory.PrettyPrint();
        var format = OptionFactory.DiffOutputFormat();
        var compareWith = OptionFactory.CompareWithFile();

        this.AddOptions(cmd, sub, app, token, outDir, escape, pretty, format, compareWith);

        this.AddExamples(cmd,
            "kmodel diff --sub-domain example --app-id 123 --api-token ABCDEF --out-dir ./raw",
            "kmodel diff --sub-domain example --app-id 123 --api-token ABCDEF --out-dir ./raw --compare-with ./raw/App123-10.metadata.json"
        );

        this.SetHandler(cmd, async parse => {
            var d = new DiffOptions {
                AppId = parse.GetValue(app),
                ApiToken = parse.GetValue(token) ?? string.Empty,
                OutputDirectory = parse.GetValue(outDir) ?? ".",
                EscapeUnicode = parse.GetValue(escape),
                Pretty = parse.GetValue(pretty),
                OutputFormat = parse.GetValue(format),
                CompareWithFile = parse.GetValue(compareWith)
            };

            var runner = this._host.Services.GetRequiredService<DiffRunner>();
            await runner.RunAsync(d, parse.GetValue(sub) ?? string.Empty);
        });

        return cmd;
    }
}
