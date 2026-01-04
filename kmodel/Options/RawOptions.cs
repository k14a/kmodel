namespace kmodel.Options;

public class RawOptions {
    public int AppId { get; set; }
    public string ApiToken { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = ".";
    public bool OverwriteExistingFiles { get; set; }
}