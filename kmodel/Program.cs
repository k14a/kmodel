using kmodel.Commands;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace kmodel;

public static class Program {
    public static async Task<int> Main(string[] args) {  
        var root = new GenerateCommandBuilder().Build();
        return await root.Parse(args).InvokeAsync();
    }
}
