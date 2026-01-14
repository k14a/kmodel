using KintoneNetLibrary.CodeGen.Application.Emitters;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using kmodel.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Interfaces;

/// <summary>
/// 型マッパーファクトリクラス
/// </summary>
/// <param name="provider"></param>
public class TypeMapperFactory(IServiceProvider provider) : ITypeMapperFactory {
    private readonly IServiceProvider _provider = provider;
    /// <summary>
    /// 型マッパーの作成
    /// </summary>
    /// <param name="lang"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public ITypeMapper Create(GenerateLanguages lang) {
        return lang switch {
            GenerateLanguages.CSharp => this._provider.GetRequiredService<CSharpTypeMapper>(),
            GenerateLanguages.Python => this._provider.GetRequiredService<PythonTypeMapper>(),
            _ => throw new NotSupportedException($"Language not supported: {lang}")
        };
    }
}