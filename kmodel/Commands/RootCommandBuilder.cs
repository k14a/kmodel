using System.CommandLine;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

public static class RootCommandBuilder {
    public static RootCommand Build(IHost host) {
        var root = new RootCommand("kmodel CLI tool") {
            GenerateCommandBuilder.Build(host),
            DiffCommandBuilder.Build(host),
            RawCommandBuilder.Build(host),
            InitCommandBuilder.Build(host)
        };

        return root;
    }
}

