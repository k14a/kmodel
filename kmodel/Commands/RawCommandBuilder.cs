using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

/// <summary>
/// RawCommandBuilder クラスは、kmodel CLI の "raw" コマンドを構築するためのクラスです。このコマンドは、Kintone アプリの生のメタデータをエクスポートする機能を提供します。ユーザーはサブドメイン、アプリID、APIトークン、出力ディレクトリ、Unicodeエスケープの有無、そして出力の整形オプションを指定して、このコマンドを実行できます。コマンドが実行されると、RawRunner クラスが呼び出され、指定されたオプションに基づいて Kintone アプリのメタデータがエクスポートされます。
/// </summary>
/// <param name="host">IHost インスタンス</param>
public class RawCommandBuilder(IHost host) : BaseCommandBuilder(host) {
    /// <summary>
    /// "raw" コマンドを構築するメソッド。コマンドには、サブドメイン、アプリID、APIトークン、出力ディレクトリ、Unicodeエスケープの有無、そして出力の整形オプションが含まれます。ユーザーがコマンドを実行すると、RawRunner クラスが呼び出され、指定されたオプションに基づいて Kintone アプリのメタデータがエクスポートされます。
    /// </summary>
    /// <returns>構築された Command インスタンス</returns>
    public Command Build() {
        var cmd = this.CreateCommand("raw", "Export raw metadata");

        var sub = OptionFactory.SubDomain();
        var app = OptionFactory.AppId();
        var token = OptionFactory.ApiToken();
        var outDir = OptionFactory.OutputDirectory("Output directory for raw metadata");
        var escape = OptionFactory.EscapeUnicode();
        var pretty = OptionFactory.PrettyPrint();

        this.AddOptions(cmd, sub, app, token, outDir, escape, pretty);

        this.AddExamples(cmd,
            "kmodel raw --sub-domain example --app-id 123 --api-token ABCDEF --out-dir ./raw",
            "kmodel raw --sub-domain example --app-id 123 --api-token ABCDEF --escape false --pretty true"
        );

        this.SetHandler(cmd, async parse => {
            var r = new RawOptions {
                AppId = parse.GetValue(app),
                ApiToken = parse.GetValue(token) ?? string.Empty,
                OutputDirectory = parse.GetValue(outDir),
                EscapeUnicode = parse.GetValue(escape),
                Pretty = parse.GetValue(pretty)
            };

            var runner = this._host.Services.GetRequiredService<RawRunner>();
            await runner.RunAsync(r, parse.GetValue(sub) ?? string.Empty);
        });

        return cmd;
    }
}
