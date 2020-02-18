using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore3xEndpointSample.OData
{
    public class DbResources
    {
        public List<string> DbTables { get; set; }
    }

    internal sealed class SqlDataSource : IDataSource
    {
        const string EdmNamespaceName = "ns";

        private readonly DbResources _tableList;

        public SqlDataSource(DbResources dbResources)
        {
            _tableList = dbResources;
        }

        public void GetModel(EdmModel model, EdmEntityContainer container)
        {
            foreach (var tableName in _tableList.DbTables)
            {
                EdmEntityType tableType = new EdmEntityType(EdmNamespaceName, tableName);
                var edmType = EdmTypeUtil.DbTypeToEdmType("nvarchar");
                if (edmType.HasValue)
                {
                    tableType.AddStructuralProperty("Column1", edmType.Value);
                    tableType.AddStructuralProperty("Column2", edmType.Value);
                }

                model.AddElement(tableType);
                container.AddEntitySet(tableName, tableType);
            }            
        }

        public void Get(ODataQueryOptions queryOptions, IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection)
        {
            var name = entityType.Definition.FullTypeName().Remove(0, EdmNamespaceName.Length + 1); // ns.
            var obj = new EdmEntityObject(entityType);
            obj.TrySetPropertyValue("Column1", "Hello World");
            obj.TrySetPropertyValue("Column2", "Hello World");

            collection.Add(obj);            
        }


        public void Get(string key, EdmEntityObject entity)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(string property, EdmEntityObject entity)
        {
            throw new NotImplementedException();
        }
    }
}
