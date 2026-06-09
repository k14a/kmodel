using kmodel.Options;
using kmodel.Services;
using KintoneNetLibrary.CodeGen.Domain.Enums;
using KintoneNetLibrary.CodeGen.Domain.Options;
using Microsoft.Extensions.Logging.Abstractions;

namespace kmodel.Tests.Services;

public class EmitterOptionsConverterTests {
    private readonly EmitterOptionsConverter _converter =
        new(NullLogger<EmitterOptionsConverter>.Instance);

    // ---------------------------------------------------------------
    // C# 変換
    // ---------------------------------------------------------------

    [Fact]
    public void ConvertToEmitterOptions_CSharp_共通プロパティが正しくマッピングされる() {
        var options = new GenerateOptions {
            AppId = 42,
            GenerateLanguage = GenerateLanguages.CSharp,
            GenerateEnums = true,
            SplitSubTableFiles = false,
            SplitHelperFiles = false,
            HeaderComment = "Auto-generated",
            ClassName = "MyApp",
        };

        var result = this._converter.ConvertToEmitterOptions(options);

        Assert.IsType<CSharpEmitterOptions>(result);
        Assert.Equal("MyApp", result.MainClassName);
        Assert.True(result.GenerateEnums);
        Assert.False(result.SplitSubTableFiles);
        Assert.False(result.SplitHelperFiles);
        Assert.Equal("Auto-generated", result.HeaderComment);
    }

    [Fact]
    public void ConvertToEmitterOptions_CSharp_言語固有プロパティが正しくマッピングされる() {
        var options = new GenerateOptions {
            AppId = 1,
            GenerateLanguage = GenerateLanguages.CSharp,
            CSharp = new CSharpOptions {
                Namespace = "MyNamespace",
                UseRecord = true,
                UsePartial = false,
                NullableEnabled = false,
                PureMode = false,
            },
        };

        var result = (CSharpEmitterOptions)this._converter.ConvertToEmitterOptions(options);

        Assert.Equal("MyNamespace", result.Namespace);
        Assert.True(result.UseRecord);
        Assert.False(result.UsePartial);
        Assert.False(result.NullableEnabled);
        Assert.True(result.UseKintoneNetLibrary);  // PureMode=false → UseKintoneNetLibrary=true
    }

    [Fact]
    public void ConvertToEmitterOptions_CSharp_PureMode有効のとき_UseKintoneNetLibraryがfalseになる() {
        var options = new GenerateOptions {
            AppId = 1,
            GenerateLanguage = GenerateLanguages.CSharp,
            CSharp = new CSharpOptions { PureMode = true },
        };

        var result = (CSharpEmitterOptions)this._converter.ConvertToEmitterOptions(options);

        Assert.False(result.UseKintoneNetLibrary);
    }

    [Fact]
    public void ConvertToEmitterOptions_CSharp_ClassNameがnullのとき_MainClassNameにAppIdが設定される() {
        var options = new GenerateOptions {
            AppId = 123,
            GenerateLanguage = GenerateLanguages.CSharp,
            ClassName = null,
        };

        var result = this._converter.ConvertToEmitterOptions(options);

        // EnsureDefaults で App{Id} が設定される
        Assert.Equal("App123", result.MainClassName);
    }

    // ---------------------------------------------------------------
    // Python 変換
    // ---------------------------------------------------------------

    [Fact]
    public void ConvertToEmitterOptions_Python_共通プロパティが正しくマッピングされる() {
        var options = new GenerateOptions {
            AppId = 99,
            GenerateLanguage = GenerateLanguages.Python,
            GenerateEnums = true,
            SplitSubTableFiles = false,
            SplitHelperFiles = false,
            HeaderComment = "# generated",
            ClassName = "MyApp",
        };

        var result = this._converter.ConvertToEmitterOptions(options);

        Assert.IsType<PythonEmitterOptions>(result);
        Assert.Equal("MyApp", result.MainClassName);
        Assert.True(result.GenerateEnums);
        Assert.False(result.SplitSubTableFiles);
        Assert.False(result.SplitHelperFiles);
        Assert.Equal("# generated", result.HeaderComment);
    }

    [Fact]
    public void ConvertToEmitterOptions_Python_言語固有プロパティが正しくマッピングされる() {
        var options = new GenerateOptions {
            AppId = 1,
            GenerateLanguage = GenerateLanguages.Python,
            Python = new PythonOptions {
                ModuleName = "my_models",
                UseTypeHint = false,
            },
        };

        var result = (PythonEmitterOptions)this._converter.ConvertToEmitterOptions(options);

        Assert.Equal("my_models", result.ModuleName);
        Assert.False(result.UseTypeHint);
    }

    [Fact]
    public void ConvertToEmitterOptions_Python_ClassNameがnullのとき_MainClassNameにAppIdが設定される() {
        var options = new GenerateOptions {
            AppId = 456,
            GenerateLanguage = GenerateLanguages.Python,
            ClassName = null,
        };

        var result = this._converter.ConvertToEmitterOptions(options);

        Assert.Equal("App456", result.MainClassName);
    }

    // ---------------------------------------------------------------
    // 未サポート言語
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(GenerateLanguages.TypeScript)]
    [InlineData(GenerateLanguages.Go)]
    [InlineData(GenerateLanguages.Java)]
    public void ConvertToEmitterOptions_未サポート言語のとき_NotSupportedExceptionをスローする(GenerateLanguages language) {
        var options = new GenerateOptions {
            AppId = 1,
            GenerateLanguage = language,
        };

        Assert.Throws<NotSupportedException>(
            () => this._converter.ConvertToEmitterOptions(options));
    }
}
