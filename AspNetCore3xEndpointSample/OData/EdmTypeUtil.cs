using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore3xEndpointSample.OData
{
    internal static class EdmTypeUtil
    {
        internal static EdmPrimitiveTypeKind? DbTypeToEdmType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "uniqueidentifier":
                    return EdmPrimitiveTypeKind.Guid;
                case "xml":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "char":
                case "nchar":
                    return EdmPrimitiveTypeKind.String;
                case "money":
                case "smallmoney":
                case "numeric":
                case "decimal":
                    return EdmPrimitiveTypeKind.Decimal;
                case "smallint":
                    return EdmPrimitiveTypeKind.Int16;
                case "int":
                    return EdmPrimitiveTypeKind.Int32;
                case "bigint":
                    return EdmPrimitiveTypeKind.Int64;
                case "tinyint":
                    return EdmPrimitiveTypeKind.Byte;
                case "float":
                    return EdmPrimitiveTypeKind.Double;
                case "real":
                    return EdmPrimitiveTypeKind.Single;
                case "bit":
                    return EdmPrimitiveTypeKind.Boolean;
                case "date":
                case "timestamp":
                case "time":
                case "smalldatetime":
                case "datetime":
                    return EdmPrimitiveTypeKind.DateTimeOffset;
                case "image":
                case "varbinary":
                case "binary":
                    return EdmPrimitiveTypeKind.Byte;
                default:
                    return null;
            }
        }
    }
}
