using KintoneNetLibrary.CodeGen.Application.Emitters;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using kmodel.Enums;
using kmodel.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Factories;

/// <summary>
/// 名前変換ファクトリクラス
/// </summary>
/// <param name="provider"></param>
public class NameConverterFactory(IServiceProvider provider) : INameConverterFactory {
    private readonly IServiceProvider _provider = provider;

    /// <summary>
    /// 名前変換の作成
    /// </summary>
    /// <param name="lang"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public INameConverter Create(GenerateLanguages lang) {
        return lang switch {
            GenerateLanguages.CSharp => this._provider.GetRequiredService<CSharpNameConverter>(),
            GenerateLanguages.Python => this._provider.GetRequiredService<PythonNameConverter>(),
            _ => throw new NotSupportedException($"Language not supported: {lang}")
        };

    }
}