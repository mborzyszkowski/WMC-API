using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WarehouseSystem.Extensions;
using WarehouseSystem.Repository;

namespace WarehouseSystem
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration) => 
            Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<WarehouseDbContext>(o =>
                    o.UseNpgsql(Configuration.GetConnectionString("WarehouseDatabase")))
                .AddScoped<DatabaseFiller>();

            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Warehouse Api", Version = "v1"});
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                })
                .AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMigration();
            
            if (env.IsDevelopment())
            {
                app
                    .UseDefaultData("true".Equals(Environment.GetEnvironmentVariable("FILLER__USE_DEFAULT_DATA")))
                    .UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseCors()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseSwagger()
                .UseSwaggerUI(options => options.SwaggerEndpoint(Path.Join(Configuration.GetValue<string>("ApiPrefix"), "/swagger/v1/swagger.json"), "Warehouse Api"));;
        }
    }
}