namespace WarehouseSystem.Core.Entity
{
    public class WmcUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string FacebookId { get; set; }
        public bool IsManager { get; set; }
    }
}