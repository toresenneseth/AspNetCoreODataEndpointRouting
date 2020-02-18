using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore3xEndpointSample.OData
{
    internal interface IDataSource
    {
        void GetModel(EdmModel model, EdmEntityContainer container);

        void Get(ODataQueryOptions queryOptions, IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection);

        void Get(string key, EdmEntityObject entity);

        object GetProperty(string property, EdmEntityObject entity);
    }
}
