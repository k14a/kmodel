using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Options;

namespace kmodel.Interfaces;

/// <summary>
/// ファイル書き込みインターフェース
/// </summary>
public interface IFileWriter {
    /// <summary>
    /// メインモデルファイルを書き込む
    /// </summary>
    /// <param name="code">生成されたコード</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリケーションID</param>
    /// <param name="revision">リビジョン番号</param>
    void WriteMainModel(string code, CodeEmitterOptions emitterOptions, FileWriterOptions fileOptions, int appId, int revision);
    /// <summary>
    /// サブテーブルモデルファイルを書き込む
    /// </summary>
    /// <param name="model">生成されたサブテーブルモデル</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリケーションID</param>
    /// <param name="revision">リビジョン番号</param>
    void WriteSubTable(GeneratedSubTableModel model, CodeEmitterOptions emitterOptions, FileWriterOptions fileOptions, int appId, int revision);
    /// <summary>
    /// ヘルパークラスファイルを書き込む
    /// </summary>
    /// <param name="model">生成されたヘルパークラスモデル</param>
    /// <param name="emitterOptions">コード生成オプション</param>
    /// <param name="fileOptions">ファイル書き込みオプション</param>
    /// <param name="appId">アプリケーションID</param>
    /// <param name="revision">リビジョン番号</param>
    void WriteHelper(GeneratedHelperClass model, CodeEmitterOptions emitterOptions, FileWriterOptions fileOptions, int appId, int revision);
    /// <summary>
    /// Raw メタデータを書き込む
    /// </summary>
    /// <param name="metadata">書き込むメタデータ</param>
    /// <param name="options">書き込みオプション</param>
    void WriteRawMetadata(KintoneAppMetadata metadata, RawOptions options);
    /// <summary>
    /// 差分結果を書き込む
    /// </summary>
    /// <param name="diffs">書き込む差分結果のリスト</param>
    /// <param name="options">書き込みオプション</param>
    void WriteDiffResults(IReadOnlyList<KintoneMetadataDiff> diffs, DiffOptions options);
}