namespace kmodel.Options;

public static class GenerateOptionsExtensions {
    public static FileWriterOptions ToFileWriterOptions(this GenerateOptions options) {
        return new FileWriterOptions {
            OutputDirectory = options.OutputDirectory,
            OverwriteExistingFiles = options.OverwriteExistingFiles
        };
    }
}