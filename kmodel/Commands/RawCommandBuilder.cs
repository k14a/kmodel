using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

public class RawCommandBuilder(IHost host) : BaseCommandBuilder(host) {
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
            "kmodel raw --subdomain example --app 123 --token ABCDEF --out ./raw",
            "kmodel raw --subdomain example --app 123 --token ABCDEF --escape false --pretty true"
        );

        this.SetHandler(cmd, async parse => {
            var r = new RawOptions {
                AppId = parse.GetValue(app),
                ApiToken = parse.GetValue(token),
                OutputDirectory = parse.GetValue(outDir),
                EscapeUnicode = parse.GetValue(escape),
                Pretty = parse.GetValue(pretty)
            };

            var runner = host.Services.GetRequiredService<RawRunner>();
            await runner.RunAsync(r, parse.GetValue(sub));
        });

        return cmd;
    }
}
