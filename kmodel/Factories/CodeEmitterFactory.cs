using KintoneNetLibrary.CodeGen.Application.Emitters;
using KintoneNetLibrary.CodeGen.Application.Interfaces;
using kmodel.Enums;
using kmodel.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace kmodel.Factories;

// /// <summary>
// /// コードエミッターファクトリクラス
// /// </summary>
// public class CodeEmitterFactory(IServiceProvider provider) : ICodeEmitterFactory {
//     private readonly IServiceProvider _provider = provider;

//     /// <summary>
//     /// コードエミッターの作成
//     /// </summary>
//     /// <param name="lang"></param>
//     /// <returns></returns>
//     /// <exception cref="NotSupportedException"></exception>
//     public ICodeEmitter Create(GenerateLanguages lang) {
//         return lang switch {
//             GenerateLanguages.CSharp => this._provider.GetRequiredService<CSharpCodeEmitter>(),
//             GenerateLanguages.Python => this._provider.GetRequiredService<PythonCodeEmitter>(),
//             _ => throw new NotSupportedException($"Language not supported: {lang}")
//         };
//     }
// }
