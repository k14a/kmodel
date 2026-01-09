using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using kmodel.Config;
using kmodel.Services;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using KintoneNetLibrary.CodeGen.Application.Services;
using KintoneNetLibrary.Application.Interfaces;
using KintoneNetLibrary.Infrastructure.Api;
using KintoneNetLibrary.CodeGen.Application.Emitters;

internal class Program {
    static async Task Main(string[] args) {
        using var host = CreateHostBuilder(args).Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("kmodel starting...");

        var runner = host.Services.GetRequiredService<KModelApplication>();
        await runner.RunAsync();

        logger.LogInformation("kmodel finished.");
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => {
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServices(services => {
                // 設定読み込み
                services.AddSingleton<ConfigLoader>();

                // アプリケーションのメイン処理
                services.AddSingleton<KModelApplication>();

                // CodeGenRunner や FileWriter などもここに追加していく
                services.AddSingleton<CodeGenRunner>();
                services.AddSingleton<IFileWriter, FileWriter>();
                services.AddSingleton<ISchemaConverter, SchemaConverter>();
                services.AddSingleton<ConfigLoader>();

                // KintoneNetLibrary.CodeGen 関連サービス
                services.AddTransient<ISchemaProvider, SchemaProvider>();
                services.AddTransient<ICodeEmitter, CSharpCodeEmitter>();
                services.AddTransient<INameConverter, NameConverter>();
                services.AddTransient<ITypeMapper, CSharpTypeMapper>();
                services.AddTransient<IXmlCommentBuilder, XmlCommentBuilder>();
                services.AddTransient<ISubTableEmitter, CSharpSubTableEmitter>();
                services.AddTransient<IHelperClassEmitter, CSharpHelperClassEmitter>();
                services.AddTransient<ISchemaProvider, SchemaProvider>();

                // KintoneNetLibrary 関連サービス
                services.AddTransient<IKintoneAppMetadataApi, KintoneAppMetadataApi>();
                services.AddHttpClient<IKintoneAppMetadataApi, KintoneAppMetadataApi>();

            });
}