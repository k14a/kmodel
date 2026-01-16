namespace kmodel.Options;

public class FileWriterOptions {
    public string OutputDirectory { get; set; } = ".";
    public bool OverwriteExistingFiles { get; set; }
}