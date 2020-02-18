using AspNetCore3xEndpointSample.OData;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore3xEndpointSample.Controllers
{
    public class HandleAllController : ODataController
    {

        public HandleAllController()
        {
        }

        public ODataServiceDocument GetServiceDocument()
        {
            return this.Request.GetModel().GenerateServiceDocument();
        }

        // Get entityset
        public IActionResult Get()
        {
            if (!IsCurrentUserAuthorized())
            {
                return Unauthorized();
            }

            // Get entity set's EDM type: A collection type.
            ODataPath path = Request.ODataFeature().Path;
            IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
            IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

            // Create an untyped collection with the EDM collection type.
            EdmEntityObjectCollection collection =
                new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

            var dataSource = GetDataSource();
            new DataSourceProvider(dataSource).Get(BuildQueryOptions(), entityType, collection);

            return Ok(collection);
        }

        public IActionResult Get(string key)
        {            
            if (!IsCurrentUserAuthorized())
            {
                return this.Unauthorized();
            }

            // Get entity type from path.
            ODataPath path = Request.ODataFeature().Path;
            IEdmEntityType entityType = (IEdmEntityType)path.EdmType;

            EdmEntityObject entity = new EdmEntityObject(entityType);

            var dataSource = GetDataSource();
            new DataSourceProvider(dataSource).Get(key, entity);

            return this.Ok(entity);
        }

        public IActionResult GetName(string key)
        {
            if (!IsCurrentUserAuthorized())
            {
                return Unauthorized();
            }

            // Get entity type from path.
            ODataPath path = Request.ODataFeature().Path;

            if (path.PathTemplate != "~/entityset/key/property")
            {
                return BadRequest("Not the correct property access request!");
            }

            dynamic property = path.Segments.Last();
            IEdmEntityType entityType = property.Property.DeclaringType as IEdmEntityType;

            // Create an untyped entity object with the entity type.
            EdmEntityObject entity = new EdmEntityObject(entityType);

            var dataSource = GetDataSource();
            var value = new DataSourceProvider(dataSource).GetProperty("Name", entity);

            if (value == null)
            {
                return NotFound();
            }

            string strValue = value as string;
            return Ok(strValue);
        }

        public IActionResult GetNavigation(string key, string navigation)
        {
            //var odataServiceEntity = GetServiceEntity();
            if (!IsCurrentUserAuthorized())
            {
                return Unauthorized();
            }

            ODataPath path = Request.ODataFeature().Path;

            if (path.PathTemplate != "~/entityset/key/navigation")
            {
                return BadRequest("Not the correct navigation property access request!");
            }

            dynamic property = path.Segments.Last();
            if (property == null)
            {
                return BadRequest("Not the correct navigation property access request!");
            }

            IEdmEntityType entityType = property.NavigationProperty.DeclaringType as IEdmEntityType;

            EdmEntityObject entity = new EdmEntityObject(entityType);

            var dataSource = GetDataSource();
            var value = dataSource.GetProperty(navigation, entity);

            if (value == null)
            {
                return NotFound();
            }

            IEdmEntityObject nav = value as IEdmEntityObject;
            if (nav == null)
            {
                return NotFound();
            }

            return Ok(nav);
        }

        private IDataSource GetDataSource()
        {
            var sourceName = Request.GetDataSource();

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

        //private ODataServiceEntity GetServiceEntity()
        //{
        //    string odataServiceName = this.Request.GetDataSource();
        //    return _repository.GetEntityByName(odataServiceName);
        //}

        private bool IsCurrentUserAuthorized()
        {
            return true;

            //if (odataServiceConfiguration.Permissions == null || odataServiceConfiguration.Permissions.Count == 0)
            //{
            //    return true;
            //}

            //string currentUserId = this.User.Identity.Name;
            //var userIdentityClaims = new HashSet<string>(UserClaims.GetUserIdentityClaimValues(this.User), StringComparer.OrdinalIgnoreCase);
            //HashSet<string> invisionUserGroupIds = null;

            //foreach (var permission in odataServiceConfiguration.Permissions)
            //{
            //    if (string.Equals(permission.UserId, currentUserId, StringComparison.OrdinalIgnoreCase) && permission.AllowRead.GetValueOrDefault(true))
            //    {
            //        return true;
            //    }
            //    else if (!string.IsNullOrWhiteSpace(permission.GroupClaimValue) && permission.AllowRead.GetValueOrDefault(true) && userIdentityClaims.Contains(permission.GroupClaimValue))
            //    {
            //        return true;
            //    }
            //    else if (!string.IsNullOrWhiteSpace(permission.SystemUserGroupId) && permission.AllowRead.GetValueOrDefault(true))
            //    {
            //        // check if the user belongs to a one of the Invision user groups
            //        if (invisionUserGroupIds == null)
            //        {
            //            invisionUserGroupIds = new HashSet<string>(_accessControlService.GetRolesIdsForCurrentUser(), StringComparer.OrdinalIgnoreCase);
            //        }

            //        if (invisionUserGroupIds.Contains(permission.SystemUserGroupId))
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;
        }

        private ODataQueryOptions BuildQueryOptions()
        {
            var path = Request.ODataFeature().Path;
            IEdmType edmType = path.Segments[0].EdmType;
            IEdmType elementType = edmType.TypeKind == EdmTypeKind.Collection
                ? (edmType as IEdmCollectionType).ElementType.Definition
                : edmType;
            ODataQueryContext queryContext = new ODataQueryContext(Request.GetModel(), elementType, path);
            ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);
            return queryOptions;
        }
    }
}
