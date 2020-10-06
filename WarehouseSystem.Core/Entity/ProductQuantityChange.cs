using System;

namespace WarehouseSystem.Core.Entity
{
    public class ProductQuantityChange
    {
        public long Id { get; set; }
        public DateTime AddDate { get; set; }
        public long Quantity { get; set; }
    }
}