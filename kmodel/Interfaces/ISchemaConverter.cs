using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.CodeGen.Domain.Schemas;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Options;

namespace kmodel.Interfaces;

/// <summary>
/// スキーマ変換インターフェース
/// </summary>
public interface ISchemaConverter {
    /// <summary>
    /// GenerateOptions を CodeEmitterOptions に変換する
    /// </summary>
    /// <param name="options">変換元の GenerateOptions インスタンス</param>
    /// <returns>変換後の CodeEmitterOptions インスタンス</returns>
    CodeEmitterOptions ConvertToEmitterOptions(GenerateOptions options);
}