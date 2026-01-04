using KintoneNetLibrary.CodeGen.Domain.Options;
using KintoneNetLibrary.CodeGen.Domain.Schemas;
using KintoneNetLibrary.Domain.Entities;
using kmodel.Options;

namespace kmodel.Services;

public interface ISchemaConverter {
    // KintoneAppSchema Convert(KintoneAppMetadata metadata);
    CodeEmitterOptions ConvertToEmitterOptions(GenerateOptions options);
}