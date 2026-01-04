using kmodel.Options;

namespace kmodel.Config;

public class KModelConfig {
    public string SubDomain{ get; set; } = string.Empty;
    public List<GenerateOptions> Generates { get; set; } = [];
    public List<RawOptions> Raws { get; set; } = [];
}