using KintoneNetLibrary.CodeGen.Application.Interfaces;

namespace kmodel.Interfaces;

/// <summary>
/// ローカル JSON ファイルからスキーマを読み込む ISchemaProvider を生成するファクトリインターフェース
/// </summary>
public interface ILocalSchemaProviderFactory {
    /// <summary>
    /// 指定されたフィールド定義ファイルを使用する LocalJsonSchemaProvider を生成する
    /// </summary>
    /// <param name="fieldsFile">フィールド定義 JSON ファイルのパス</param>
    /// <returns>ISchemaProvider インスタンス</returns>
    ISchemaProvider Create(string fieldsFile);
}
