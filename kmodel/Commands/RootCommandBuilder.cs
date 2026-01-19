using System.CommandLine;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

public static class RootCommandBuilder {
    public static RootCommand Build(IHost host) {
        var root = new RootCommand("kmodel CLI tool") {
            new GenerateCommandBuilder(host).Build(),
            new DiffCommandBuilder(host).Build(),
            new RawCommandBuilder(host).Build(),
            InitCommandBuilder.Build()
        };

        return root;
    }
}

