using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Core.Helpers;
using WarehouseSystem.Query;
using WarehouseSystem.Repository;
using WarehouseSystem.Security;
using WarehouseSystem.Services;

namespace WarehouseSystem.Controllers
{
    [Route("access")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly WarehouseDbContext _context;
        private readonly JwtTokenService _tokenService;

        public AccessController(WarehouseDbContext context, JwtTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("facebook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResult>> AuthenticateWithFacebook([FromBody] FacebookAuthForm facebookAuth, CancellationToken token)
        {
            var validator = new FacebookAuthForm.Validator();
            var validationResult = await validator.ValidateAsync(facebookAuth, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }
            
            var client = new HttpClient();
            var result = await client.GetStringAsync("https://graph.facebook.com/me?fields=id,name&access_token=" + facebookAuth.Token);

            if (result.Contains("error"))
            {
                return BadRequest();
            }

            var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(result);
            
            var wmcUser = await _context.WmcUser
                .FirstOrDefaultAsync(wu => wu.FacebookId.Equals(facebookUser.Id), token);

            if (wmcUser == null)
            {
                wmcUser = new WmcUser
                {
                    Name = facebookUser.Name,
                    FacebookId = facebookUser.Id
                };
                await _context.WmcUser.AddAsync(wmcUser, token);
                await _context.SaveChangesAsync(token);
            }

            var resultToken = await CreateToken(wmcUser, token);

            return Ok(resultToken);
        }

        [HttpPost("wmc")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResult>> AuthenticateWithWmc([FromBody] WmcAuthForm wmcAuth, CancellationToken token)
        {
            var validator = new WmcAuthForm.Validator();
            var validationResult = await validator.ValidateAsync(wmcAuth, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var passwordHash = Sha512Helper.GetHash(wmcAuth.Password);
            
            var wmcUser = await _context.WmcUser
                .FirstOrDefaultAsync(wu => wu.Name.Equals(wmcAuth.UserName) && wu.Password.Equals(passwordHash), token);

            if (wmcUser == null)
            {
                return BadRequest();
            }

            var resultToken = await CreateToken(wmcUser, token);

            return Ok(resultToken);
        }

        [HttpPost("token/refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResult>> RefreshToken([FromBody] RefreshTokenForm refreshToken, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(refreshToken.Token))
            {
                return BadRequest();
            }

            var wmcUser = await _context.WmcUser
                .FirstOrDefaultAsync(wu => refreshToken.Token.Equals(wu.RefreshToken), token);

            if (wmcUser == null)
            {
                return BadRequest();
            }

            var resultToken = await CreateToken(wmcUser, token);

            return Ok(resultToken);
        }

        [HttpGet("userInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<UserInfoResult>> GetUserInfo([FromServices] UserInfo userInfo, CancellationToken token)
        {
            var user = await _context.WmcUser
                .FirstAsync(wu => wu.Id.Equals(userInfo.UserId), token);
            
            var userInfoResult = new UserInfoResult
            {
                Username = user.Name,
                Roles = user.IsManager ? new List<string> { "employee", "manager" } : new List<string> { "employee" },
            };

            return Ok(userInfoResult);
        }

        private async Task<TokenResult> CreateToken(WmcUser wmcUser, CancellationToken token)
        {
            var identity = CreateClaimsIdentity(wmcUser);

            var resultToken = _tokenService.GenerateToken(identity, string.IsNullOrWhiteSpace(wmcUser.RefreshToken) ? Sha512Helper.GetRandomHash() : wmcUser.RefreshToken);

            if (string.IsNullOrWhiteSpace(wmcUser.RefreshToken))
            {
                wmcUser.RefreshToken = resultToken.RefreshToken;
                await _context.SaveChangesAsync(token);
            }

            return resultToken;
        }
        
        private static ClaimsIdentity CreateClaimsIdentity(WmcUser wmcUser)
        {
            ClaimsIdentity identity;
            
            if (wmcUser.IsManager)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(WarehouseClaims.UserId, wmcUser.Id.ToString()),
                    new Claim(WarehouseClaims.Manager, "true"), 
                });
            }
            else
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(WarehouseClaims.UserId, wmcUser.Id.ToString()),
                });
            }

            return identity;
        }
    }
}