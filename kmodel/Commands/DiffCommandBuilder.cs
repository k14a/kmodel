using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

public static class DiffCommandBuilder {
    public static Command Build(IHost host) {
        var cmd = new Command("diff", "Compare metadata and output differences");

        var sub = new Option<string>("--subdomain") { Required = true };
        var app = new Option<int>("--app") { Required = true };
        var token = new Option<string>("--token") { Required = true };
        var outDir = new Option<string>("--out") { DefaultValueFactory = _ => "." };

        cmd.Add(sub);
        cmd.Add(app);
        cmd.Add(token);
        cmd.Add(outDir);

        cmd.SetAction(async result => {
            var d = new DiffOptions {
                AppId = result.GetValue(app),
                ApiToken = result.GetValue(token),
                OutputDirectory = result.GetValue(outDir)
            };

            var runner = host.Services.GetRequiredService<CodeGenRunner>();
            await runner.RunDiffAsync(d, result.GetValue(sub));
        });

        return cmd;
    }
}