using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace kmodel.Config;

public class ConfigLoader(ILogger<ConfigLoader> logger) {
    private readonly ILogger<ConfigLoader> _logger = logger;

    public KModelConfig Load(string path) {
        this._logger.LogInformation("Loading config file: {Path}", path);

        if (!File.Exists(path)){ throw new FileNotFoundException($"Config file not found: {path}"); }

        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        var config = JsonSerializer.Deserialize<KModelConfig>(json, options)
                     ?? throw new InvalidOperationException("Failed to deserialize config file.");

        this._logger.LogInformation("Config loaded. {Count} generate entries.", config.Generates.Count);

        return config;
    }
}
