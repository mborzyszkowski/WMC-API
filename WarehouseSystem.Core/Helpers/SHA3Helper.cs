using System.Text;
using SHA3.Net;

namespace WarehouseSystem.Core.Helpers
{
    public static class Sha3Helper
    {
        public static string GetHash(string data)
        {
            using var sha3Algorithm = Sha3.Sha3512();
            return Encoding.UTF8.GetString(sha3Algorithm.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }
    }
}