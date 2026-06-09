using kmodel.Config;
using kmodel.Enums;
using kmodel.Options;
using KintoneNetLibrary.CodeGen.Domain.Enums;

namespace kmodel.Tests.Config;

public class ConfigToOptionsTests {

    // ---------------------------------------------------------------
    // GenerateConfig
    // ---------------------------------------------------------------

    [Fact]
    public void GenerateConfig_ToOptions_全プロパティが正しくマッピングされる() {
        var csharp = new CSharpOptions {
            Namespace = "MyNs",
            UseRecord = true,
            UsePartial = false,
            NullableEnabled = false,
            PureMode = true,
        };
        var python = new PythonOptions { ModuleName = "my_module", UseTypeHint = false };
        var fileWriter = new FileWriterOptions { OutputDirectory = "/output", OverwriteExistingFiles = true };

        var config = new GenerateConfig {
            AppId = 42,
            ApiToken = "token",
            GenerateEnums = true,
            GenerateLanguage = GenerateLanguages.Python,
            SplitSubTableFiles = false,
            SplitHelperFiles = false,
            HeaderComment = "header",
            ClassName = "MyClass",
            CSharp = csharp,
            Python = python,
            FileWriter = fileWriter,
            FieldsFile = "fields.json",
            ExportNameTableTemplate = "template.json",
            NameTable = "names.csv",
        };

        var result = config.ToOptions();

        Assert.Equal(42, result.AppId);
        Assert.Equal("token", result.ApiToken);
        Assert.True(result.GenerateEnums);
        Assert.Equal(GenerateLanguages.Python, result.GenerateLanguage);
        Assert.False(result.SplitSubTableFiles);
        Assert.False(result.SplitHelperFiles);
        Assert.Equal("header", result.HeaderComment);
        Assert.Equal("MyClass", result.ClassName);
        Assert.Same(csharp, result.CSharp);
        Assert.Same(python, result.Python);
        Assert.Same(fileWriter, result.FileWriter);
        Assert.Equal("fields.json", result.FieldsFile);
        Assert.Equal("template.json", result.ExportNameTableTemplate);
        Assert.Equal("names.csv", result.NameTable);
    }

    // ---------------------------------------------------------------
    // RawConfig
    // ---------------------------------------------------------------

    [Fact]
    public void RawConfig_ToOptions_全プロパティが正しくマッピングされる() {
        var config = new RawConfig {
            AppId = 10,
            ApiToken = "rawtoken",
            OutputDirectory = "/raw/output",
            EscapeUnicode = false,
            OverwriteExistingFiles = true,
            Pretty = true,
        };

        var result = config.ToOptions();

        Assert.Equal(10, result.AppId);
        Assert.Equal("rawtoken", result.ApiToken);
        Assert.Equal("/raw/output", result.OutputDirectory);
        Assert.False(result.EscapeUnicode);
        Assert.True(result.OverwriteExistingFiles);
        Assert.True(result.Pretty);
    }

    // ---------------------------------------------------------------
    // DiffConfig
    // ---------------------------------------------------------------

    [Fact]
    public void DiffConfig_ToOptions_全プロパティが正しくマッピングされる() {
        var config = new DiffConfig {
            AppId = 99,
            ApiToken = "difftoken",
            OutputDirectory = "/diff/output",
            OverwriteExistingFiles = true,
            CompareWithFile = "base.json",
            OutputFormat = DiffOutputFormats.Markdown,
            DiffOutputFile = "diff_{appId}.txt",
            FailIfNoBaseFile = true,
            EscapeUnicode = false,
            Pretty = true,
        };

        var result = config.ToOptions();

        Assert.Equal(99, result.AppId);
        Assert.Equal("difftoken", result.ApiToken);
        Assert.Equal("/diff/output", result.OutputDirectory);
        Assert.True(result.OverwriteExistingFiles);
        Assert.Equal("base.json", result.CompareWithFile);
        Assert.Equal(DiffOutputFormats.Markdown, result.OutputFormat);
        Assert.Equal("diff_{appId}.txt", result.DiffOutputFile);
        Assert.True(result.FailIfNoBaseFile);
        Assert.False(result.EscapeUnicode);
        Assert.True(result.Pretty);
    }
}
