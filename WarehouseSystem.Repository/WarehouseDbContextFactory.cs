using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WarehouseSystem.Repository
{
    public class WarehouseDbContextFactory :IDesignTimeDbContextFactory<WarehouseDbContext>
    {
        public WarehouseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder.UseNpgsql("Host=db.postgres;Port=5432;Database=postgres;Username=postgres;Password=awhousepi");
            
            return new WarehouseDbContext(optionsBuilder.Options);
        }
    }
}