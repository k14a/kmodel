using System.CommandLine;
using kmodel.Enums;

namespace kmodel.Commands;

public static class OptionFactory {
    public static Option<string> SubDomain() =>
        new("--subdomain") {
            Required = true,
            Description = "Kintone subdomain (e.g., 'example' for example.cybozu.com)"
        };

    public static Option<int> AppId() =>
        new("--app") {
            Required = true,
            Description = "Kintone App ID"
        };

    public static Option<string> ApiToken() =>
        new("--token") {
            Required = true,
            Description = "Kintone API Token"
        };

    public static Option<string?> OutputDirectory(string description) =>
        new("--out") {
            Required = false,
            DefaultValueFactory = _ => null,
            Description = description
        };

    public static Option<string> Language() =>
        new("--lang") {
            Required = false,
            DefaultValueFactory = _ => "CSharp",
            Description = "Programming language for code generation"
        };

    public static Option<bool> EscapeUnicode() =>
        new("--escape") {
            Required = false,
            DefaultValueFactory = _ => true,
            Description = "Escape field codes in output"
        };

    public static Option<bool> PrettyPrint() =>
        new("--pretty") {
            Required = false,
            DefaultValueFactory = _ => false,
            Description = "Pretty print the output JSON"
        };

    public static Option<DiffOutputFormats> DiffOutputFormat() =>
        new("--format") {
            Required = false,
            DefaultValueFactory = _ => DiffOutputFormats.Text,
            Description = "Output format for diff (Text or Json or Markdown)"
        };
}
