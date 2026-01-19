using System.CommandLine;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

public class GenerateCommandBuilder(IHost host) : BaseCommandBuilder(host) {
    public Command Build() {
        var cmd = this.CreateCommand("generate", "Generate model code from Kintone app schema");

        var sub = OptionFactory.SubDomain();
        var app = OptionFactory.AppId();
        var token = OptionFactory.ApiToken();
        var lang = OptionFactory.Language();
        var outDir = OptionFactory.OutputDirectory("Output directory for generated code");

        this.AddOptions(cmd, sub, app, token, lang, outDir);

        this.AddExamples(cmd,
            "kmodel generate --subdomain example --app 123 --token ABCDEF --lang CSharp --out ./Models",
            "kmodel generate --subdomain example --app 456 --token XYZ --lang TypeScript --out ./ts"
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
