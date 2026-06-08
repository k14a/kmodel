using KintoneNetLibrary.CodeGen.Domain.Options;
using kmodel.Options;

namespace kmodel.Interfaces;

/// <summary>
/// GenerateOptions を CodeEmitterOptions へ変換するインターフェース
/// </summary>
public interface IEmitterOptionsConverter {
    /// <summary>
    /// GenerateOptions を CodeEmitterOptions に変換する
    /// </summary>
    /// <param name="options">変換元の GenerateOptions インスタンス</param>
    /// <returns>変換後の CodeEmitterOptions インスタンス</returns>
    CodeEmitterOptions ConvertToEmitterOptions(GenerateOptions options);
}