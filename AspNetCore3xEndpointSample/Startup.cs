// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using AspNetCore3xEndpointSample.Models;
using AspNetCore3xEndpointSample.OData;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore3xEndpointSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CustomerOrderContext>(opt => opt.UseLazyLoadingProxies().UseInMemoryDatabase("CustomerOrderList"));
            services.AddOData();
            services.AddControllers();            

            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseODataBatching();

            app.UseEndpoints(endpoints =>
            {                
                //endpoints.Select().Expand().Filter().OrderBy().MaxTop(100).Count();                

                var routeBuilder = endpoints.MapODataServiceRoute("odataPrefix", "odata/{dataSource}", containerBuilder =>
                {
                    containerBuilder.AddService(Microsoft.OData.ServiceLifetime.Scoped, typeof(IEdmModel), sp =>
                    {
                        var serviceScope = sp.GetRequiredService<HttpRequestScope>();                        
                        IEdmModel model = DataSourceProvider.GetEdmModel(serviceScope.HttpRequest);
                        return model;
                    });

                    containerBuilder.AddService(Microsoft.OData.ServiceLifetime.Scoped, typeof(IEnumerable<IODataRoutingConvention>), sp =>
                    {
                        IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
                        routingConventions.Insert(0, new MatchAllRoutingConvention());
                        return routingConventions.ToList().AsEnumerable();
                    });                    

                });               
            });            
        }
    }
}
