using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WarehouseSystem.Core.Helpers;
using WarehouseSystem.Options;
using WarehouseSystem.Query;

namespace WarehouseSystem.Security
{
    public class JwtTokenService
    {
        private readonly SecurityOptions _securityOptions;

        public JwtTokenService(IOptions<SecurityOptions> securityOptions)
        {
            _securityOptions = securityOptions.Value;
        }
        
        public TokenResult GenerateToken(ClaimsIdentity identity)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_securityOptions.Secret);

            var expiration = DateTime.UtcNow.AddMinutes(_securityOptions.Expiration);
            
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = handler.CreateJwtSecurityToken(descriptor);

            return new TokenResult
            {
                Token = handler.WriteToken(jwtToken),
                RefreshToken = Sha512Helper.GetRandomHash(),
                ExpirationDate = expiration
            };
        }
    }
}