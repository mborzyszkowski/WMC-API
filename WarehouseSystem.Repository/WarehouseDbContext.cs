using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Core.Entity;

namespace WarehouseSystem.Repository
{
    public class WarehouseDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<WmcUser> WmcUser { get; set; }
        
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseSnakeCaseNamingConvention();
    }
}