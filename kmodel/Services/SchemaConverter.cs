using KintoneNetLibrary.CodeGen.Domain.Schemas;
using KintoneNetLibrary.Domain.Entities;

namespace kmodel.Services;

public class SchemaConverter : ISchemaConverter {
    public KintoneAppSchema Convert(KintoneAppMetadata metadata) {
        var schema = new KintoneAppSchema {
            AppId = metadata.AppId,
            Fields = [],
            Subtables = []
        };

        foreach (var f in metadata.Fields) {
            if (f.Type == KintoneFieldType.SubTable) {
                // サブテーブルとして登録
                schema.Subtables.Add(new KintoneSubtableSchema {
                    FieldCode = f.Code,
                    Label = f.Label,
                    Fields = f.SubFields?.Select(sf => new KintoneFieldSchema {
                        FieldCode = sf.Code,
                        Label = sf.Label,
                        FieldType = sf.Type,
                        Required = sf.Required,
                        Options = sf.Options==null ?[] : [.. sf.Options]
                    }).ToList() ?? []
                });

            } else {
                // 通常フィールドとして登録
                schema.Fields.Add(new KintoneFieldSchema {
                    FieldCode = f.Code,
                    Label = f.Label,
                    FieldType = f.Type,
                    Required = f.Required,
                    Options = f.Options == null ? [] : [.. f.Options]
                });
            }
        }

        return schema;
    }
}