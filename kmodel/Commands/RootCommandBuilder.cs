using System.CommandLine;
using Microsoft.Extensions.Hosting;

namespace kmodel.Commands;

/// <summary>
/// ルートコマンドのビルダークラス。kmodel CLI のエントリーポイントとなるコマンドを構築します。Generate、Diff、Raw、Init の各サブコマンドをルートコマンドに追加し、ユーザーが適切なコマンドを選択できるようにします。
/// </summary>
public static class RootCommandBuilder {
    /// <summary>
    /// ルートコマンドを構築する静的メソッド。Generate、Diff、Raw、Init の各サブコマンドをルートコマンドに追加し、ユーザーが適切なコマンドを選択できるようにします。例えば、ユーザーが "kmodel generate" と入力すると、Generate コマンドが実行されます。
    /// </summary>
    /// <param name="host">IHost インスタンス</param>
    /// <returns>構築された RootCommand インスタンス</returns>
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

