using System;
using System.Collections.Generic;

namespace WarehouseSystem.Core.Entity
{
    public class Product
    {
        public long Id { get; set; }
        public DateTime AddDate { get; set; }
        public WmcUser AddUser { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public double Price { get; set; }
        public double? PriceUsd { get; set; }
        public List<ProductQuantityChange> QuantityChanges { get; set; }
        
        public static Product CreateNewProduct(long id, string manufacturerName, string modelName, double price, double? priceUsd, WmcUser addUser)
        {
            var result = CreateNewProduct(manufacturerName, modelName, price, priceUsd, addUser);
            result.Id = id;
            return result;
        }

        public static Product CreateNewProduct(string manufacturerName, string modelName, double price, double? priceUsd, WmcUser addUser) =>
            new Product
            {
                ManufacturerName = manufacturerName,
                ModelName = modelName,
                Price = price,
                PriceUsd = priceUsd,
                AddDate = DateTime.Now,
                AddUser = addUser,
                QuantityChanges = new List<ProductQuantityChange>(),
            };

        public void UpdateProduct(Product product)
        {
            ManufacturerName = product.ManufacturerName;
            ModelName = product.ModelName;
            Price = product.Price;
            if (product.PriceUsd.HasValue) PriceUsd = product.PriceUsd;
        }
    }
}