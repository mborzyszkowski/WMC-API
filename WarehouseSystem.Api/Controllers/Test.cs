using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Repository;

namespace WarehouseSystem.Controllers
{
    [ApiController]
    [Route("test")]
    public class Test : ControllerBase
    {
        private readonly WarehouseDbContext _context;

        public Test(WarehouseDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets product from warehouse by id, only for presentation purposes
        /// </summary>
        /// <response code="200">Successfully retrieved Products</response>
        /// <response code="401">Only for authorise user</response>
        /// <returns>Whole product entity with all childs</returns>
        [HttpGet("products/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<Product>> GetProduct(long productId, CancellationToken token) =>
            await _context.Products
                .Include(p => p.QuantityChanges)
                .Include(p => p.QuantityChanges)
                .ThenInclude(q => q.AddUser)
                .FirstOrDefaultAsync(p => p.Id == productId, token);
    }
}