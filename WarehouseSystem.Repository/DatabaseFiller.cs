using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseSystem.Core.Entity;

namespace WarehouseSystem.Repository
{
    public class DatabaseFiller
    {
        private readonly WarehouseDbContext _context;

        public DatabaseFiller(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task FillDatabase()
        {
            List<Product> products = new List<Product>
            {
                new Product
                {
                    ManufacturerName = "Samsung", 
                    ModelName = "Galaxy S9",
                    Price = 3499,
                    AddDate = new DateTime(2020, 5, 9),
                    QuantityChanges = new List<ProductQuantityChange>
                    {
                        new ProductQuantityChange
                        {
                            Quantity = 3,
                            AddDate = new DateTime(2020, 5, 10),
                        },
                        new ProductQuantityChange
                        {
                            Quantity = -1,
                            AddDate = new DateTime(2020, 6, 10),
                        },
                    }
                },
                new Product
                {
                    ManufacturerName = "Huawei", 
                    ModelName = "P9",
                    Price = 1500,
                    AddDate = new DateTime(2020, 7, 9),
                    QuantityChanges = new List<ProductQuantityChange>
                    {
                        new ProductQuantityChange
                        {
                            Quantity = 6,
                            AddDate = new DateTime(2020, 7, 10),
                            
                        },
                        new ProductQuantityChange
                        {
                            Quantity = 5,
                            AddDate = new DateTime(2020, 7, 11),
                            
                        },
                        new ProductQuantityChange
                        {
                            Quantity = -1,
                            AddDate = new DateTime(2020, 7, 12),
                            
                        },
                    }
                },
            };
            
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
}