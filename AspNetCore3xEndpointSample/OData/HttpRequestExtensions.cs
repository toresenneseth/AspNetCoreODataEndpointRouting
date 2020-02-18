using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore3xEndpointSample.OData
{
    public static class HttpRequestExtensions
    {        
        public static string GetDataSource(this HttpRequest request)
        {
            return "MySource"; // TODO, extract the actual source from the request.            
        }
    }
}
