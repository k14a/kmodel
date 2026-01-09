using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Models;
using kmodel.Options;

namespace kmodel.Interfaces;

/// <summary>
/// ファイル書き込みインターフェース
/// </summary>
public interface IFileWriter {
    /// <summary>
    /// メインモデルファイルを書き込む
    /// </summary>
    /// <param name="code"></param>
    /// <param name="options"></param>
    /// <param name="appId"></param>
    /// <param name="revision"></param>
    void WriteMainModel(string code, CodeEmitterOptions options, int appId, int revision);
    /// <summary>
    /// サブテーブルモデルファイルを書き込む
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="appId"></param>
    /// <param name="revision"></param>
    void WriteSubTable(GeneratedSubTableModel model, CodeEmitterOptions options, int appId, int revision);
    /// <summary>
    /// ヘルパークラスファイルを書き込む
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="appId"></param>
    /// <param name="revision"></param>
    void WriteHelper(GeneratedHelperClass model, CodeEmitterOptions options, int appId, int revision);
    /// <summary>
    /// Raw メタデータを書き込む
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="options"></param>
    void WriteRawMetadata(KintoneAppMetadata metadata, RawOptions options);
    /// <summary>
    /// 差分結果を書き込む
    /// </summary>
    /// <param name="diffs"></param>
    /// <param name="options"></param>
    void WriteDiffResults(List<DiffResult> diffs, DiffOptions options);
}