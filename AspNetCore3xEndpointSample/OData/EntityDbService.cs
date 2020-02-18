//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AspNetCore3xEndpointSample.OData
//{
//    public sealed class EntityDbService : IEntityDbService
//    {
//        private string _solutionDbConnectionString;

//        public EntityDbService(string solutionDbConnectionString)
//        {
//            _solutionDbConnectionString = solutionDbConnectionString;
//        }

//        public void ExecuteReadQuery(Entity entity, ODataQueryOptions queryOptions, Action<SqlDataReader> rowRead)
//        {
//            var sqlCommandConfig = BuildSqlCommandConfig(entity, queryOptions);
//            using (var sqlConn = new SqlConnection(sqlCommandConfig.ConnectionString))
//            {
//                using (var cmd = new SqlCommand(sqlCommandConfig.Query, sqlConn))
//                {
//                    cmd.Parameters.AddRange(sqlCommandConfig.SqlParameters.ToArray());
//                    sqlConn.Open();
//                    using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
//                    {
//                        while (reader.Read())
//                        {
//                            rowRead(reader);
//                        }
//                    }
//                }
//            }
//        }

//        public List<SqlColumn> GetTableSchema(Entity entity)
//        {
//            var entityStorageInfo = ResolveEntityStorage(entity);
//            return new SqlExecuter(entityStorageInfo.ConnectionString).GetColumns(entityStorageInfo.DbObjectName);
//        }

//        private (string Query, string ConnectionString, List<SqlParameter> SqlParameters) BuildSqlCommandConfig(Entity entity, ODataQueryOptions queryOptions)
//        {
//            var sqlParams = new List<SqlParameter>();
//            var cxt = queryOptions.Context;
//            var entityStorageInfo = ResolveEntityStorage(entity);

//            string cmdSql = "SELECT {0} {1} FROM {2} {3} {4} {5} {6}";
//            string top = string.Empty;
//            string skip = string.Empty;
//            string fetch = string.Empty;

//            if (queryOptions.Count == null && queryOptions.Top != null)
//            {
//                if (queryOptions.Skip != null)
//                {
//                    skip = string.Format("OFFSET {0} ROWS", queryOptions.Skip.RawValue); ;
//                    fetch = string.Format("FETCH NEXT {0} ROWS ONLY", queryOptions.Top.RawValue);
//                    top = string.Empty;
//                }
//                else
//                {
//                    top = string.Concat("TOP ", queryOptions.Top.RawValue);
//                }
//            }

//            var query = string.Format(cmdSql
//                , top
//                , queryOptions.ParseSelect()
//                , entityStorageInfo.DbObjectName
//                , queryOptions.ParseFilter(sqlParams)
//                , queryOptions.ParseOrderBy()
//                , skip
//                , fetch);

//            return (query, entityStorageInfo.ConnectionString, sqlParams);
//        }

//        private (string DbObjectName, string ConnectionString) ResolveEntityStorage(Entity entity)
//        {
//            // SourceObjectId can be either an actual id or an @Object[...].Id token
//            string contentId = entity.SourceObjectId;
//            if (Directives.ExpressionContainsDirective(entity.SourceObjectId, Directives.Object))
//            {
//                contentId = DirectivesHelper.ProcessDirectives(new DirectiveResolutionSettings { Expression = new StringBuilder(entity.SourceObjectId) }).GetResult();
//            }

//            using (var db = new InvisionModel())
//            {
//                string dbObjectName = db.GetStorageTableName(contentId);
//                string connectionString = db.GetObjectConnectionString();

//                return (dbObjectName, connectionString);
//            }
//        }
//    }
//}
