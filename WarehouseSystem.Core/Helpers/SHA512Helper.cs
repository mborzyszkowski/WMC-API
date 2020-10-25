using System;
using System.Security.Cryptography;
using System.Text;

namespace WarehouseSystem.Core.Helpers
{
    public static class Sha512Helper
    {
        public static string GetHash(string data)
        {
            using var sha256Hash = SHA512.Create();
            
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));  
   
            var builder = new StringBuilder();  
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }  
            
            return builder.ToString();
        }
        
        public static string GetRandomHash() {
            const int length = 100;
            var stringBuilder = new StringBuilder();  
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var flt = random.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(25 * flt));
                var letter = Convert.ToChar(shift + 65);
                stringBuilder.Append(letter);  
            }

            return GetHash(stringBuilder.ToString());
        }
    }
}