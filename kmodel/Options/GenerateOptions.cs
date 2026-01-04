namespace kmodel.Options;

public class GenerateOptions {
    public int AppId { get; set; }
    public string ApiToken { get; set; } = string.Empty;
    public string Namespace { get; set; } = "KintoneModels";
    public string? ClassName { get; set; }
    public bool UseRecord { get; set; }
    public bool PureMode { get; set; }
    public bool GenerateEnums { get; set; }
    public string OutputDirectory { get; set; } = ".";
    public bool OverwriteExistingFiles { get; set; }
}