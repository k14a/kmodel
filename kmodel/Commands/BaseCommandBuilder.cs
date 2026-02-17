using System.CommandLine;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

/// <summary>
/// コマンドビルダークラスの基底クラス。各コマンドビルダークラスはこのクラスを継承し、共通のコマンド構築ロジックを提供します。例えば、コマンドの作成、オプションの追加、ハンドラーの設定、そして例の追加などの共通機能をこのクラスで定義し、各コマンドビルダークラスはこれらの機能を利用して独自のコマンドを構築します。
/// </summary>
/// <param name="host">ホストインスタンス</param>
public abstract class BaseCommandBuilder(IHost host) {
    /// <summary>
    /// IHost インスタンスを保持するフィールド。各コマンドビルダークラスはこのフィールドを使用して、コマンドのハンドラー内で必要なサービスを取得することができます。例えば、GenerateRunner や DiffRunner などのサービスをこのホストから取得して、コマンドの実行に利用することができます。
    /// </summary>
    protected readonly IHost _host = host;

    /// <summary>
    /// コマンド名と説明を受け取り、Command を生成する
    /// </summary>
    /// <param name="name">コマンド名</param>
    /// <param name="description">コマンドの説明</param>
    /// <returns>生成された Command インスタンス</returns>
    protected Command CreateCommand(string name, string description) => new(name, description);

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
    /// <param name="cmd">Action を設定するコマンド</param>
    /// <param name="handler">コマンドが実行されたときに呼び出されるハンドラー</param>
    protected void SetHandler(Command cmd, Func<ParseResult, Task> handler) {
        cmd.SetAction(handler);
    }

    /// <summary>
    /// コマンドに例を追加する
    /// </summary>
    /// <param name="cmd">例を追加するコマンド</param>
    /// <param name="examples">追加する例の配列</param>
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
