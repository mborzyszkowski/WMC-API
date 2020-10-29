using System;
using WarehouseSystem.Core.Entity;

namespace WarehouseSystem.Query
{
    public class ProductAction
    {
        public enum ActionType
        {
            Add, Update, Delete, ChangeQuantity
        }
    
        public ActionType Action { get; set; }
        // All existing on API, if empty then product created but not sync with api
        public long Id { get; set; }
        // On update/create
        public string ManufacturerName { get; set; }
        // On update/create
        public string ModelName { get; set; }
        // On update/create
        public double Price { get; set; }
        // On quantity change
        public long QuantityChange { get; set; }

        public bool IsNewItem => Id < 0;
        
        public bool IsExistingItem => Id > 0;

        public bool IsAddAction => Action == ActionType.Add;
        
        public bool IsUpdateAction => Action == ActionType.Update;
        
        public bool IsDeleteAction => Action == ActionType.Delete;
        
        public bool IsChangeQuantityAction => Action == ActionType.ChangeQuantity;

        public bool IsPositiveChangeQuantity => IsChangeQuantityAction && QuantityChange > 0;

        public bool IsNegativeChangeQuantity => IsChangeQuantityAction && QuantityChange < 0;

        public Product CreateNewProductWithFakeId(long id, WmcUser user) =>
            Product.CreateNewProduct(id, ManufacturerName, ModelName, Price, user);

        public Product CreateNewProduct(WmcUser user) =>
            Product.CreateNewProduct(ManufacturerName, ModelName, Price, user);

        // TODO: add validation rules for each item
    }
}