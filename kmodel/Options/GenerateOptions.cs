namespace kmodel.Options;

public class GenerateOptions {
    public int AppId { get; set; }
    public string Namespace { get; set; } = "KintoneModels";
    public string? ClassName { get; set; }
    public bool UseRecord { get; set; }
    public bool PureMode { get; set; }
    public bool GenerateEnums { get; set; }
}