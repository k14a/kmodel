using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using KintoneNetLibrary.Application.Interfaces;
using KintoneNetLibrary.Infrastructure.Api;
using KintoneNetLibrary.CodeGen.Application.Emitters;
using KintoneNetLibrary.CodeGen.Infrastructure.Services;
using kmodel.Config;
using kmodel.Services;
using kmodel.Interfaces;
using KintoneNetLibrary.CodeGen.Infrastructure.Factories;
using System.CommandLine;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using kmodel.Commands;

internal class Program {
    static async Task<int> Main(string[] args) {
        using var host = CreateHostBuilder(args).Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("kmodel starting...");

        try {
            var runner = host.Services.GetRequiredService<KModelApplication>();
            var configLoader = host.Services.GetRequiredService<ConfigLoader>();

            // ============================================================
            // 引数なし → config.json を使って現行の RunAsync() を実行
            // ============================================================
            if (args.Length == 0) {
                logger.LogInformation("kmodel starting with config.json...");
                await runner.RunAsync();
                logger.LogInformation("kmodel finished.");
                return 0;
            }

            // ============================================================
            // 引数あり → System.CommandLine でパースしてコマンド実行
            // ============================================================
            var rootCommand = RootCommandBuilder.Build(host);
            return await rootCommand.Parse(args).InvokeAsync();

        } finally {
            logger.LogInformation("kmodel finished.");
        }
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

                // KintoneNetLibrary.CodeGen 関連サービス
                services.AddTransient<ISchemaProvider, SchemaProvider>();
                services.AddTransient<IXmlCommentBuilder, CSharpXmlCommentBuilder>();
                services.AddTransient<ISubTableEmitter, CSharpSubTableEmitter>();
                services.AddTransient<IHelperClassEmitter, CSharpHelperClassEmitter>();

                // Codeエミッターファクトリ
                services.AddTransient<CSharpCodeEmitter>();
                services.AddTransient<PythonCodeEmitter>();
                services.AddSingleton<ICodeEmitterFactory, CodeEmitterFactory>();

                // NameConverterファクトリ
                services.AddTransient<CSharpNameConverter>();
                services.AddTransient<PythonNameConverter>();
                services.AddSingleton<INameConverterFactory, NameConverterFactory>();

                // TypeMapperファクトリ
                services.AddTransient<CSharpTypeMapper>();
                services.AddTransient<PythonTypeMapper>();
                services.AddSingleton<ITypeMapperFactory, TypeMapperFactory>();

                // KintoneNetLibrary 関連サービス
                services.AddTransient<IKintoneAppMetadataApi, KintoneAppMetadataApi>();
                services.AddHttpClient<IKintoneAppMetadataApi, KintoneAppMetadataApi>();

            });
}