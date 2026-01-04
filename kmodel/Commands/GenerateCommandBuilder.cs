using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using KintoneNetLibrary.Infrastructure.Api;
using KintoneNetLibrary.CodeGen.Application.Emitters;
using kmodel.Options;
using kmodel.Services;
using KintoneNetLibrary.Domain.Access;

namespace kmodel.Commands;

public class GenerateCommandBuilder {
    public RootCommand Build() {
        // // 1. オプションの定義
        // var appId = new Option<int>("--app", "Kintone App ID") { Required = true }; // 最新では Required ではなく IsRequired
        // var ns = new Option<string>("--namespace", "Namespace for generated models") { 
        //     DefaultValueFactory = _ => "KintoneModels" 
        // };
        // var className = new Option<string?>("--class-name");
        // var pure = new Option<bool>("--pure");
        // var useRecord = new Option<bool>("--record");

        // // 2. コマンドの作成
        // var generate = new Command("generate", "Generate C# models from Kintone app") { 
        //     appId, ns, className, pure, useRecord 
        // };

        // // 3. ハンドラーの設定 (NamingConventionBinder を使用)
        // // 引数名は Option の名前に合わせる必要があります (--app -> app, --namespace -> ns)
        // generate.Handler = CommandHandler.Create<int, string, string?, bool, bool>(
        //     async (app, @namespace, className, pure, record) => { // パラメータ名は Option 名に準拠
        //         var opts = new GenerateOptions {
        //             AppId = app,
        //             Namespace = @namespace, // ns オプションは namespace 引数にバインドされる
        //             ClassName = className,
        //             PureMode = pure,
        //             UseRecord = record
        //         };

        //         var generator = BuildGenerateCommand();
        //         await generator.ExecuteAsync(opts);
        //     }
        // );

        // // 4. RootCommand への登録
        // // ★重要: new Command(...) を再度作らず、上記で作った generate をそのまま入れる
        // var root = new RootCommand("kmodel CLI") { generate };

        // return root;
        throw new NotImplementedException();
    }

    // private static GenerateCommand BuildGenerateCommand() {
    //     var access = new ApiTokenAccess("domain", "token");
    //     var http = new HttpClient();
    //     var api = new KintoneAppMetadataApi(access, http);

    //     return new GenerateCommand(
    //         api,
    //         new SchemaConverter(),
    //         new CSharpCodeEmitter(
    //             new NameConverter(),
    //             new CSharpTypeMapper(),
    //             new XmlCommentBuilder(),
    //             new CSharpSubtableEmitter(new NameConverter(), new CSharpTypeMapper(), new XmlCommentBuilder()),
    //             new CSharpHelperClassEmitter()
    //         ),
    //         new FileWriter()
    //     );
    // }
}