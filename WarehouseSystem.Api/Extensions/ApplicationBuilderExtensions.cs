using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarehouseSystem.Repository;

namespace WarehouseSystem.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                    using var context = serviceScope.ServiceProvider.GetService<WarehouseDbContext>();

                    context.Database.Migrate();

                    done = true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return app;
        }
        
        public static IApplicationBuilder UseDefaultData(this IApplicationBuilder app, bool useDefaultData)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<WarehouseDbContext>();
            var filler = serviceScope.ServiceProvider.GetService<DatabaseFiller>(); 
            
            context.Database.Migrate();
            if (useDefaultData && !context.Products.Any())
            {
                filler.FillDatabase().Wait();
            }
            
            return app;
        }
    }
}