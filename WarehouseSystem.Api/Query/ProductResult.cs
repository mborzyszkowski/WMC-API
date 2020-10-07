using System;

namespace WarehouseSystem.Query
{
    public class ProductResult
    {
        public long Id { get; set; }
        public DateTime AddDate { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public double Price { get; set; }
        public long Quantity { get; set; }
    }
}