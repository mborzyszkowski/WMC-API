using System;

namespace WarehouseSystem.Core.Entity
{
    public class ProductQuantityChange
    {
        public long Id { get; set; }
        public DateTime AddDate { get; set; }
        public WmcUser AddUser { get; set; }
        public long Quantity { get; set; }

        public static ProductQuantityChange CreateQuantityChange(long quantityChange, WmcUser addUser) =>
            new ProductQuantityChange
            {
                Quantity = quantityChange,
                AddDate = DateTime.Now,
                AddUser = addUser,
            };
    }
}