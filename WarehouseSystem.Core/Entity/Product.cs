using System;
using System.Collections.Generic;

namespace WarehouseSystem.Core.Entity
{
    public class Product
    {
        public long Id { get; set; }
        public DateTime AddDate { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public double Price { get; set; }
        public List<ProductQuantityChange> QuantityChanges { get; set; }

        public static Product CreateNewProduct(string manufacturerName, string modelName, double price) =>
            new Product
            {
                ManufacturerName = manufacturerName,
                ModelName = modelName,
                Price = price,
                AddDate = DateTime.Now,
                QuantityChanges = new List<ProductQuantityChange>(),
            };

        public void UpdateProduct(Product product)
        {
            ManufacturerName = product.ManufacturerName;
            ModelName = product.ModelName;
            Price = product.Price;
        }
    }
}