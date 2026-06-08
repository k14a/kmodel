using KintoneNetLibrary.CodeGen.Application.Interfaces;
using KintoneNetLibrary.CodeGen.Infrastructure.Services;
using KintoneNetLibrary.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using kmodel.Interfaces;

namespace kmodel.Services;

/// <summary>
/// LocalJsonSchemaProvider を生成するファクトリ。Infrastructure 層の具象クラスの生成責務を Application 層から分離する。
/// </summary>
/// <param name="fieldParser">フィールドパーサー</param>
/// <param name="converter">メタデータコンバーター</param>
/// <param name="loggerFactory">ロガーファクトリ</param>
public class LocalSchemaProviderFactory(
    IKintoneFieldParser fieldParser,
    IMetadataConverter converter,
    ILoggerFactory loggerFactory) : ILocalSchemaProviderFactory {

    private readonly IKintoneFieldParser _fieldParser = fieldParser;
    private readonly IMetadataConverter _converter = converter;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    /// <inheritdoc/>
    public ISchemaProvider Create(string fieldsFile) {
        return new LocalJsonSchemaProvider(
            fieldsFile,
            this._fieldParser,
            this._converter,
            this._loggerFactory.CreateLogger<LocalJsonSchemaProvider>());
    }
}
