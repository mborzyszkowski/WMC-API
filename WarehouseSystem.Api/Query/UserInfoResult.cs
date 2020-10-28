using System.Collections.Generic;

namespace WarehouseSystem.Query
{
    public class UserInfoResult
    {
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}