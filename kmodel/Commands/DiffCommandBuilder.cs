using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

public class DiffCommandBuilder(IHost host) : BaseCommandBuilder(host) {
    public Command Build() {
        var cmd = this.CreateCommand("diff", "Compare metadata and output differences");

        var sub = OptionFactory.SubDomain();
        var app = OptionFactory.AppId();
        var token = OptionFactory.ApiToken();
        var outDir = OptionFactory.OutputDirectory("Output directory for metadata diff");
        var escape = OptionFactory.EscapeUnicode();
        var pretty = OptionFactory.PrettyPrint();
        var format = OptionFactory.DiffOutputFormat();

        this.AddOptions(cmd, sub, app, token, outDir, escape, pretty, format);

        this.AddExamples(cmd,
            "kmodel diff --subdomain example --app 123 --token ABCDEF --out ./raw",
            "kmodel diff --subdomain example --app 123 --token ABCDEF --out ./raw --compare-with ./raw/App123-10.metadata.json"
        );

        this.SetHandler(cmd, async parse => {
            var d = new DiffOptions {
                AppId = parse.GetValue(app),
                ApiToken = parse.GetValue(token),
                OutputDirectory = parse.GetValue(outDir) ?? ".",
                EscapeUnicode = parse.GetValue(escape),
                Pretty = parse.GetValue(pretty),
                OutputFormat = parse.GetValue(format)
            };

            var runner = this._host.Services.GetRequiredService<DiffRunner>();
            await runner.RunAsync(d, parse.GetValue(sub));
        });

        return cmd;
    }
}
