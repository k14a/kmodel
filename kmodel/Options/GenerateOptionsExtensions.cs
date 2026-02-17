namespace kmodel.Options;

/// <summary>
/// GenerateOptions を言語ごとの CodeEmitterOptions に変換する拡張メソッドクラス。GenerateRunner で使用され、ユーザーが指定した GenerateOptions をコード生成のためのオプションに変換します。
/// 例えば、GenerateOptions の OutputDirectory や OverwriteExistingFiles を FileWriterOptions に変換するための ToFileWriterOptions メソッドなどが含まれます。これにより、GenerateRunner 内で GenerateOptions を直接 FileWriterOptions に変換して使用することができます。
/// 将来的には、他のオプションへの変換メソッドもここに追加される可能性があります。
/// </summary>
public static class GenerateOptionsExtensions {
    /// <summary>
    /// GenerateOptions を FileWriterOptions に変換する。GenerateOptions の OutputDirectory と OverwriteExistingFiles を FileWriterOptions の対応するプロパティにマッピングします。これにより、GenerateRunner 内で GenerateOptions を直接 FileWriterOptions に変換して使用することができます。
    /// </summary>
    /// <param name="options">変換元の GenerateOptions</param>
    /// <returns>変換後の FileWriterOptions</returns>
    public static FileWriterOptions ToFileWriterOptions(this GenerateOptions options) {
        return new FileWriterOptions {
            OutputDirectory = options.OutputDirectory,
            OverwriteExistingFiles = options.OverwriteExistingFiles
        };
    }
}