using System.CommandLine;
using Microsoft.Extensions.Hosting;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Commands;

public static class GenerateCommandBuilder {
    public static Command Build(IHost host) {
        var cmd = new Command("generate", "Generate model code from Kintone app schema");

        var sub = new Option<string>("--subdomain") { Required = true };
        var app = new Option<int>("--app") { Required = true };
        var token = new Option<string>("--token") { Required = true };
        var lang = new Option<string>("--lang") { DefaultValueFactory = _ => "CSharp" };
        var outDir = new Option<string>("--out") { DefaultValueFactory = _ => "." };

        cmd.Add(sub);
        cmd.Add(app);
        cmd.Add(token);
        cmd.Add(lang);
        cmd.Add(outDir);

        cmd.SetAction(async result => {
            var g = new GenerateOptions {
                AppId = result.GetValue(app),
                ApiToken = result.GetValue(token),
                GenerateLanguage = Enum.Parse<GenerateLanguages>(result.GetValue(lang), true),
                OutputDirectory = result.GetValue(outDir)
            };

            var runner = host.Services.GetRequiredService<CodeGenRunner>();
            await runner.RunAsync(g, result.GetValue(sub));
        });

        return cmd;
    }
}