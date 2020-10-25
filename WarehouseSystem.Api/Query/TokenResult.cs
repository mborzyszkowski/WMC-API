using System;

namespace WarehouseSystem.Query
{
    public class TokenResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}