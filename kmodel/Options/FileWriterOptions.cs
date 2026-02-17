namespace kmodel.Options;

/// <summary>
/// ファイル書き込みオプション。FileWriter で使用され、コード生成されたファイルの出力先や上書きの有無などを指定します。OutputDirectory プロパティで出力先ディレクトリを指定し、OverwriteExistingFiles プロパティで既存のファイルを上書きするかどうかを指定します。
/// 例えば、OutputDirectory を "." に設定するとカレントディレクトリに出力されます。OutputDirectory を "output" に設定すると、プロジェクトルートの output ディレクトリに出力されます。OverwriteExistingFiles を true に設定すると、同名のファイルが存在する場合に上書きされます。false に設定すると、同名のファイルが存在する場合はスキップされます。
/// </summary>
public class FileWriterOptions {
    /// <summary>
    /// 出力先ディレクトリ。コード生成されたファイルの出力先を指定します。例えば、"." に設定するとカレントディレクトリに出力されます。 "output" に設定すると、プロジェクトルートの output ディレクトリに出力されます。
    /// </summary>
    public string OutputDirectory { get; set; } = ".";
    /// <summary>
    /// 既存のファイルを上書きするかどうかを指定します。true に設定すると、同名のファイルが存在する場合に上書きされます。false に設定すると、同名のファイルが存在する場合はスキップされます。
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }
}