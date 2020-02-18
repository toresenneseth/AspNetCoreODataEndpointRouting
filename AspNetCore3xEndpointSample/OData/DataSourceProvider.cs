using AspNetCore3xEndpointSample.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore3xEndpointSample.OData
{
    internal class DataSourceProvider
    {
        private IDataSource _dataSource;

        public DataSourceProvider(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public static IEdmModel GetEdmModel(HttpRequest request)
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("ns", "container");
            model.AddElement(container);

            var dataSource = GetDataSource(request);
            dataSource.GetModel(model, container);
            return model;
        }

        private static IDataSource GetDataSource(HttpRequest request)
        {
            var sourceName = request.GetDataSource();

            // TODO, get the DbResource based on the sourceName
            var dbResources = new DbResources
            {
                DbTables = new List<string>
                {
                    "Table1"
                }
            };

            var dataSource = new SqlDataSource(dbResources);
            return dataSource;
        }

        public void Get(
            ODataQueryOptions queryOptions,
            IEdmEntityTypeReference entityType,
            EdmEntityObjectCollection collection)
        {
            _dataSource.Get(queryOptions, entityType, collection);
        }

        public void Get(string key, EdmEntityObject entity)
        {
            _dataSource.Get(key, entity);
        }

        public object GetProperty(string property, EdmEntityObject entity)
        {
            return _dataSource.GetProperty(property, entity);
        }
    }
}
