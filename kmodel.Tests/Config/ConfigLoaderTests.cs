using kmodel.Config;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace kmodel.Tests.Config;

public class ConfigLoaderTests : IDisposable {
    private readonly ConfigLoader _loader;
    private readonly string _tempDir;

    public ConfigLoaderTests() {
        this._loader = new(NullLogger<ConfigLoader>.Instance);
        this._tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(this._tempDir);
    }

    public void Dispose() {
        if (Directory.Exists(this._tempDir)) {
            Directory.Delete(this._tempDir, true);
        }
    }

    private string WriteJson(string json) {
        var path = Path.Combine(this._tempDir, Path.GetRandomFileName() + ".json");
        File.WriteAllText(path, json);
        return path;
    }

    // ---------------------------------------------------------------
    // 正常系
    // ---------------------------------------------------------------

    [Fact]
    public void Load_正常なJSON_camelCaseキーがデシリアライズされる() {
        var path = this.WriteJson("""
            {
              "subDomain": "example",
              "generates": [
                {
                  "appId": 42,
                  "apiToken": "mytoken",
                  "generateEnums": true
                }
              ]
            }
            """);

        var result = this._loader.Load(path);

        Assert.Equal("example", result.SubDomain);
        Assert.Single(result.Generates);
        Assert.Equal(42, result.Generates[0].AppId);
        Assert.Equal("mytoken", result.Generates[0].ApiToken);
        Assert.True(result.Generates[0].GenerateEnums);
    }

    [Fact]
    public void Load_enumが文字列で記述されている_正しくデシリアライズされる() {
        var path = this.WriteJson("""
            {
              "generates": [
                {
                  "appId": 1,
                  "generateLanguage": "Python"
                }
              ]
            }
            """);

        var result = this._loader.Load(path);

        Assert.Equal(GenerateLanguages.Python, result.Generates[0].GenerateLanguage);
    }

    [Fact]
    public void Load_コメント付きJSON_コメントがスキップされてデシリアライズされる() {
        var path = this.WriteJson("""
            {
              // 行コメント
              "subDomain": "example", /* ブロックコメント */
              "generates": []
            }
            """);

        var result = this._loader.Load(path);

        Assert.Equal("example", result.SubDomain);
    }

    [Fact]
    public void Load_末尾カンマあり_デシリアライズされる() {
        var path = this.WriteJson("""
            {
              "subDomain": "example",
              "generates": [],
            }
            """);

        var result = this._loader.Load(path);

        Assert.Equal("example", result.SubDomain);
    }

    [Fact]
    public void Load_generates空配列_空リストが返される() {
        var path = this.WriteJson("""
            {
              "subDomain": "example",
              "generates": []
            }
            """);

        var result = this._loader.Load(path);

        Assert.Empty(result.Generates);
    }

    // ---------------------------------------------------------------
    // 異常系
    // ---------------------------------------------------------------

    [Fact]
    public void Load_ファイルが存在しない_FileNotFoundExceptionをスローする() {
        var notExistPath = Path.Combine(this._tempDir, "notexist.json");

        Assert.Throws<FileNotFoundException>(() => this._loader.Load(notExistPath));
    }

    [Fact]
    public void Load_JSONがnullリテラル_InvalidOperationExceptionをスローする() {
        // Deserialize<KModelConfig>("null") → null → ?? throw InvalidOperationException
        var path = this.WriteJson("null");

        Assert.Throws<InvalidOperationException>(() => this._loader.Load(path));
    }
}
