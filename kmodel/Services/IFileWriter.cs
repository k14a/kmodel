using KintoneNetLibrary.CodeGen.Domain.Models;
using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Options;

namespace kmodel.Services;

public interface IFileWriter {
    void WriteMainModel(string code, CodeEmitterOptions options, int appId, int revision);
    void WriteSubTable(GeneratedSubTableModel model, CodeEmitterOptions options, int appId, int revision);
    void WriteHelper(GeneratedHelperClass model, CodeEmitterOptions options, int appId, int revision);
    void WriteRawMetadata(KintoneAppMetadata metadata, RawOptions options);
}