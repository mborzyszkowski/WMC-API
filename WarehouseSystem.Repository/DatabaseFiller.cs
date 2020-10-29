using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Core.Helpers;

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
            var users = new List<WmcUser>
            {
                new WmcUser
                {
                    Name = "manager",
                    Password = Sha512Helper.GetHash("manager"),
                    RefreshToken = Sha512Helper.GetRandomHash(),
                    IsManager = true,
                },
                new WmcUser
                {
                    Name = "dealer",
                    Password = Sha512Helper.GetHash("dealer"),
                    RefreshToken = Sha512Helper.GetRandomHash(),
                    IsManager = true,
                }
            };

            await _context.WmcUser.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var user = await _context.WmcUser.FirstAsync();
            
            var products = new List<Product>
            {
                new Product
                {
                    ManufacturerName = "Samsung", 
                    ModelName = "Galaxy S9",
                    Price = 3499,
                    AddDate = new DateTime(2020, 5, 9),
                    AddUser = user,
                    QuantityChanges = new List<ProductQuantityChange>
                    {
                        new ProductQuantityChange
                        {
                            Quantity = 3,
                            AddDate = new DateTime(2020, 5, 10),
                            AddUser = user,
                        },
                        new ProductQuantityChange
                        {
                            Quantity = -1,
                            AddDate = new DateTime(2020, 6, 10),
                            AddUser = user,
                        },
                    }
                },
                new Product
                {
                    ManufacturerName = "Huawei", 
                    ModelName = "P9",
                    Price = 1500,
                    AddDate = new DateTime(2020, 7, 9),
                    AddUser = user,
                    QuantityChanges = new List<ProductQuantityChange>
                    {
                        new ProductQuantityChange
                        {
                            Quantity = 6,
                            AddDate = new DateTime(2020, 7, 10),
                            AddUser = user,
                        },
                        new ProductQuantityChange
                        {
                            Quantity = 5,
                            AddDate = new DateTime(2020, 7, 11),
                            AddUser = user,
                        },
                        new ProductQuantityChange
                        {
                            Quantity = -1,
                            AddDate = new DateTime(2020, 7, 12),
                            AddUser = user,
                        },
                    }
                }, 
                new Product
                {
                    ManufacturerName = "To delete", 
                    ModelName = "TD",
                    Price = 401234,
                    AddDate = new DateTime(2020, 7, 9),
                    AddUser = user,
                    QuantityChanges = new List<ProductQuantityChange>(),
                },
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
}