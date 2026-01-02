using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using KintoneNetLibrary.Infrastructure.Api;
using KintoneNetLibrary.CodeGen.Application.Emitters;
using kmodel.Options;
using kmodel.Services;
using KintoneNetLibrary.Domain.Entities;
using System.ComponentModel;
using System.CommandLine.Parsing;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Commands;

public class GenerateCommandBuilder {
    public RootCommand Build() {
        var appId = new Option<int>("--app", "Kintone App ID") { Required = true };
        var ns = new Option<string>("--namespace", "Namespace for generated models") { 
            DefaultValueFactory = _ => "KintoneModels" 
        };
        var className = new Option<string?>("--class-name");
        var pure = new Option<bool>("--pure");
        var useRecord = new Option<bool>("--record");

        var generate = new Command("generate", "Generate C# models from Kintone app") { 
            appId, ns, className, pure, useRecord 
        };

        // SetHandler を使用（型安全なバインド）
        generate.SetHandler(async (int appVal, string nsVal, string? classVal, bool pureVal, bool recordVal) => {
            var opts = new GenerateOptions {
                AppId = appVal,
                Namespace = nsVal,
                ClassName = classVal,
                PureMode = pureVal,
                UseRecord = recordVal
            };

            var generator = BuildGenerateCommand();
            await generator.ExecuteAsync(opts);
        }, appId, ns, className, pure, useRecord);

        // root に generate コマンドを追加
        var root = new RootCommand("kmodel CLI") { generate };

        return root;
    }
    // public RootCommand Build() {
    //     var appId = new Option<int>("--app", "Kintone App ID") { Required = true };
    //     var ns = new Option<string>("--namespace", "Namespace for generated models") { DefaultValueFactory = _ => "KintoneModels" };
    //     var className = new Option<string?>("--class-name");
    //     var pure = new Option<bool>("--pure");
    //     var useRecord = new Option<bool>("--record");

    //     var generate = new Command("generate", "Generate C# models from Kintone app") { appId, ns, className, pure, useRecord };

    //     var root = new RootCommand("kmodel CLI") { new Command("generate", "Generate C# models from Kintone app") { appId, ns, className, pure, useRecord } };

    //     generate.Handler = CommandHandler.Create<int, string, string?, bool, bool>(
    //         async (app, ns, className, pure, useRecord) => {
    //             var opts = new GenerateOptions {
    //                 AppId = app,
    //                 Namespace = ns,
    //                 ClassName = className,
    //                 PureMode = pure,
    //                 UseRecord = useRecord
    //             };

    //             var generator = BuildGenerateCommand();
    //             await generator.ExecuteAsync(opts);
    //         }
    //     );

    //     return root;
    // }

    private static GenerateCommand BuildGenerateCommand() {
        var access = new ApiTokenAccess("domain", "token");
        var http = new HttpClient();
        var api = new KintoneAppMetadataApi(access, http);

        return new GenerateCommand(
            api,
            new SchemaConverter(),
            new CSharpCodeEmitter(
                new NameConverter(),
                new CSharpTypeMapper(),
                new XmlCommentBuilder(),
                new CSharpSubtableEmitter(new NameConverter(), new CSharpTypeMapper(), new XmlCommentBuilder()),
                new CSharpHelperClassEmitter()
            ),
            new FileWriter()
        );
    }
}