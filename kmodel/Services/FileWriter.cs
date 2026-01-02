using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;

namespace kmodel.Services;

public class FileWriter : IFileWriter {
    public void WriteMainModel(string code, CodeEmitterOptions options) {
        var file = $"{options.MainClassName}.cs";
        File.WriteAllText(file, code);
        Console.WriteLine($"  - {file}");
    }

    public void WriteSubtable(GeneratedSubtableModel model, CodeEmitterOptions options) {
        var file = $"{model.ClassName}.cs";
        File.WriteAllText(file, model.Code);
        Console.WriteLine($"  - {file}");
    }

    public void WriteHelper(GeneratedHelperClass model, CodeEmitterOptions options) {
        var file = $"{model.ClassName}.cs";
        File.WriteAllText(file, model.Code);
        Console.WriteLine($"  - {file}");
    }
}