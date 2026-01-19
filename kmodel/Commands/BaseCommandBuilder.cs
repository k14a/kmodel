using System.CommandLine;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

public abstract class BaseCommandBuilder(IHost host) {
    protected readonly IHost _host = host;

    /// <summary>
    /// コマンド名と説明を受け取り、Command を生成する
    /// </summary>
    protected Command CreateCommand(string name, string description)
        => new(name, description);

    /// <summary>
    /// コマンドに Option を追加する（可変長）
    /// </summary>
    protected void AddOptions(Command cmd, params Option[] options) {
        foreach (var opt in options) {
            cmd.Add(opt);
        }
    }

    /// <summary>
    /// コマンドの Action を設定する
    /// </summary>
    protected void SetHandler(Command cmd, Func<ParseResult, Task> handler) {
        cmd.SetAction(handler);
    }

    protected void AddExamples(Command cmd, params string[] examples) {
        if (examples.Length == 0) { return; }

        var sb = new StringBuilder();
        sb.AppendLine(cmd.Description);
        sb.AppendLine();
        sb.AppendLine("### Examples");

        foreach (var ex in examples) {
            sb.AppendLine();
            sb.AppendLine("```bash");
            sb.AppendLine(ex);
            sb.AppendLine("```");
        }

        cmd.Description = sb.ToString();
    }
}
