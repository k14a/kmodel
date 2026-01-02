using KintoneNetLibrary.CodeGen.Domain.Schemas;
using KintoneNetLibrary.Domain.Entities;

namespace kmodel.Services;

public interface ISchemaConverter {
    KintoneAppSchema Convert(KintoneAppMetadata metadata);
}