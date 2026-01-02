using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;

namespace kmodel.Services;

public interface IFileWriter {
    void WriteMainModel(string code, CodeEmitterOptions options);
    void WriteSubtable(GeneratedSubtableModel model, CodeEmitterOptions options);
    void WriteHelper(GeneratedHelperClass model, CodeEmitterOptions options);
}