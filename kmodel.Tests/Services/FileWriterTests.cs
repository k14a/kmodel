using kmodel.Options;
using kmodel.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace kmodel.Tests.Services;

public class FileWriterTests : IDisposable {
    private readonly FileWriter _writer;
    private readonly string _tempDir;

    public FileWriterTests() {
        this._writer = new(NullLogger<FileWriter>.Instance);
        this._tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(this._tempDir);
    }

    public void Dispose() {
        if (Directory.Exists(this._tempDir)) {
            Directory.Delete(this._tempDir, true);
        }
    }

    private FileWriterOptions MakeOptions(bool overwrite = false)
        => new() { OutputDirectory = this._tempDir, OverwriteExistingFiles = overwrite };

    // ---------------------------------------------------------------
    // WriteMainModel / WriteFile
    // ---------------------------------------------------------------

    [Fact]
    public void WriteMainModel_新規ファイル_書き込まれる() {
        this._writer.WriteMainModel("Test.cs", "hello", this.MakeOptions());

        Assert.Equal("hello", File.ReadAllText(Path.Combine(this._tempDir, "Test.cs")));
    }

    [Fact]
    public void WriteMainModel_上書き禁止_既存ファイルをスキップする() {
        var path = Path.Combine(this._tempDir, "Test.cs");
        File.WriteAllText(path, "original");

        this._writer.WriteMainModel("Test.cs", "new content", this.MakeOptions(overwrite: false));

        Assert.Equal("original", File.ReadAllText(path));
    }

    [Fact]
    public void WriteMainModel_上書き許可_既存ファイルを上書きする() {
        var path = Path.Combine(this._tempDir, "Test.cs");
        File.WriteAllText(path, "original");

        this._writer.WriteMainModel("Test.cs", "new content", this.MakeOptions(overwrite: true));

        Assert.Equal("new content", File.ReadAllText(path));
    }

    [Fact]
    public void WriteMainModel_不正ファイル名_ファイルが書き込まれない() {
        // GetInvalidFileNameChars() に含まれる文字 (環境非依存)
        var invalidFileName = $"invalid{Path.GetInvalidFileNameChars()[0]}file.cs";

        this._writer.WriteMainModel(invalidFileName, "content", this.MakeOptions());

        Assert.Empty(Directory.GetFiles(this._tempDir));
    }

    [Fact]
    public void WriteMainModel_存在しないディレクトリ_ディレクトリが作成されファイルが書き込まれる() {
        var subDir = Path.Combine(this._tempDir, "sub");
        var options = new FileWriterOptions { OutputDirectory = subDir, OverwriteExistingFiles = false };

        this._writer.WriteMainModel("Test.cs", "content", options);

        Assert.True(File.Exists(Path.Combine(subDir, "Test.cs")));
    }

    // ---------------------------------------------------------------
    // WriteDiffResults
    // ---------------------------------------------------------------

    [Fact]
    public void WriteDiffResults_DiffOutputFileがnull_AppIdがファイル名に使われる() {
        var options = new DiffOptions { AppId = 42, OutputDirectory = this._tempDir, DiffOutputFile = null };

        this._writer.WriteDiffResults("content", options);

        Assert.True(File.Exists(Path.Combine(this._tempDir, "App42.diff.txt")));
    }

    [Fact]
    public void WriteDiffResults_DiffOutputFileにappIdプレースホルダー_AppIdに置換される() {
        var options = new DiffOptions { AppId = 42, OutputDirectory = this._tempDir, DiffOutputFile = "diff_{appId}.txt" };

        this._writer.WriteDiffResults("content", options);

        Assert.True(File.Exists(Path.Combine(this._tempDir, "diff_42.txt")));
    }

    [Fact]
    public void WriteDiffResults_DiffOutputFileにプレースホルダーなし_そのまま使われる() {
        var options = new DiffOptions { AppId = 42, OutputDirectory = this._tempDir, DiffOutputFile = "my_diff.txt" };

        this._writer.WriteDiffResults("content", options);

        Assert.True(File.Exists(Path.Combine(this._tempDir, "my_diff.txt")));
    }

    [Fact]
    public void WriteDiffResults_コンテンツが正しく書き込まれる() {
        var options = new DiffOptions { AppId = 1, OutputDirectory = this._tempDir };

        this._writer.WriteDiffResults("diff content", options);

        Assert.Equal("diff content", File.ReadAllText(Path.Combine(this._tempDir, "App1.diff.txt")));
    }
}
