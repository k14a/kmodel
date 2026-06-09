using kmodel.Services;

namespace kmodel.Tests.Services;

public class GenerateRunnerCsvTests {

    // ---------------------------------------------------------------
    // ParseDelimitedLine
    // ---------------------------------------------------------------

    [Fact]
    public void ParseDelimitedLine_通常フィールド_正しく分割される() {
        var result = GenerateRunner.ParseDelimitedLine("a,b,c", ',');

        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void ParseDelimitedLine_ダブルクォートで囲まれたフィールド_クォートが除去される() {
        var result = GenerateRunner.ParseDelimitedLine("\"hello\",world", ',');

        Assert.Equal(["hello", "world"], result);
    }

    [Fact]
    public void ParseDelimitedLine_ダブルクォートのエスケープ_クォートが復元される() {
        var result = GenerateRunner.ParseDelimitedLine("\"a\"\"b\",c", ',');

        Assert.Equal(["a\"b", "c"], result);
    }

    [Fact]
    public void ParseDelimitedLine_区切り文字を含むフィールド_クォート内の区切り文字は無視される() {
        var result = GenerateRunner.ParseDelimitedLine("\"a,b\",c", ',');

        Assert.Equal(["a,b", "c"], result);
    }

    [Fact]
    public void ParseDelimitedLine_空フィールド_空文字列が返される() {
        var result = GenerateRunner.ParseDelimitedLine(",,", ',');

        Assert.Equal(["", "", ""], result);
    }

    [Fact]
    public void ParseDelimitedLine_TSV区切り_正しく分割される() {
        var result = GenerateRunner.ParseDelimitedLine("a\tb\tc", '\t');

        Assert.Equal(["a", "b", "c"], result);
    }

    // ---------------------------------------------------------------
    // Escape
    // ---------------------------------------------------------------

    [Fact]
    public void Escape_通常の値_そのまま返される() {
        var result = GenerateRunner.Escape("abc", ',');

        Assert.Equal("abc", result);
    }

    [Fact]
    public void Escape_区切り文字を含む値_クォートで囲まれる() {
        var result = GenerateRunner.Escape("a,b", ',');

        Assert.Equal("\"a,b\"", result);
    }

    [Fact]
    public void Escape_改行LFを含む値_クォートで囲まれる() {
        var result = GenerateRunner.Escape("a\nb", ',');

        Assert.Equal("\"a\nb\"", result);
    }

    [Fact]
    public void Escape_改行CRを含む値_クォートで囲まれる() {
        var result = GenerateRunner.Escape("a\rb", ',');

        Assert.Equal("\"a\rb\"", result);
    }

    [Fact]
    public void Escape_クォートを含む値_ダブルクォートにエスケープされクォートで囲まれる() {
        var result = GenerateRunner.Escape("a\"b", ',');

        Assert.Equal("\"a\"\"b\"", result);
    }

    [Fact]
    public void Escape_null値_空文字列が返される() {
        var result = GenerateRunner.Escape(null, ',');

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Escape_空文字列_空文字列が返される() {
        var result = GenerateRunner.Escape("", ',');

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Escape_TSV区切り文字を含む値_クォートで囲まれる() {
        var result = GenerateRunner.Escape("a\tb", '\t');

        Assert.Equal("\"a\tb\"", result);
    }
}
