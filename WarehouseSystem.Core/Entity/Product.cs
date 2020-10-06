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
    }
}