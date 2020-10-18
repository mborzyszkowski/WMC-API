using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Query;
using WarehouseSystem.Repository;
using WarehouseSystem.Security;
using WarehouseSystem.Services;

namespace WarehouseSystem.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all products from warehouse
        /// </summary>
        /// <response code="200">Successfully retrieved Products</response>
        /// <response code="401">Only for authorise user</response>
        /// <returns>All products info in warehouse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductResult>>> SearchAllProducts(CancellationToken token) =>
            await _context.Products
                .Include(p => p.QuantityChanges)
                .SelectMany(
                    p => p.QuantityChanges.DefaultIfEmpty(),
                    (p, c) => new {p.Id, p.AddDate, p.ManufacturerName, p.ModelName, p.Price, c.Quantity })
                .GroupBy(p => new { p.Id, p.AddDate, p.ManufacturerName, p.ModelName, p.Price })
                .Select(p => new ProductResult
                {
                    Id = p.Key.Id,
                    AddDate = p.Key.AddDate,
                    ManufacturerName = p.Key.ManufacturerName,
                    ModelName = p.Key.ModelName,
                    Price = p.Key.Price,
                    Quantity = p.Sum(c => c.Quantity),
                })
                .ToListAsync(token);

        /// <summary>
        /// Gets product from warehouse by id
        /// </summary>
        /// <response code="200">Successfully retrieved Product</response>
        /// <response code="401">Only for authorise user</response>
        /// <response code="404">Product with given id not found</response>
        /// <returns>Product from warehouse by id</returns>
        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<ProductResult>> GetProduct(long productId, CancellationToken token)
        {
            var product = await _context.Products
                .Include(p => p.QuantityChanges)
                .SelectMany(
                    p => p.QuantityChanges.DefaultIfEmpty(),
                    (p, c) => new {p.Id, p.AddDate, p.ManufacturerName, p.ModelName, p.Price, c.Quantity})
                .GroupBy(p => new {p.Id, p.AddDate, p.ManufacturerName, p.ModelName, p.Price})
                .Select(p => new ProductResult
                {
                    Id = p.Key.Id,
                    AddDate = p.Key.AddDate,
                    ManufacturerName = p.Key.ManufacturerName,
                    ModelName = p.Key.ModelName,
                    Price = p.Key.Price,
                    Quantity = p.Sum(c => c.Quantity),
                })
                .FirstOrDefaultAsync(p => p.Id == productId, token);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Adds new product to warehouse
        /// </summary>
        /// <response code="201">Successfully added Product</response>
        /// <response code="400">Product form malformed</response>
        /// <response code="401">Only for authorise user</response>
        /// <returns>New product</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> AddProduct([FromBody] ProductForm productForm, [FromServices] UserInfo userInfo, CancellationToken token)
        {
            var validator = new ProductForm.Validator();
            var validationResult = await validator.ValidateAsync(productForm, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var user = await _context.WmcUser
                .FirstAsync(wu => wu.Id == userInfo.UserId, token);
            
            var newProduct = Product.CreateNewProduct(productForm.ManufacturerName, productForm.ModelName, productForm.Price.Value, user);

            await _context.Products.AddAsync(newProduct, token);
            await _context.SaveChangesAsync(token);

            return Created("product", _mapper.Map<ProductResult>(newProduct));
        }

        /// <summary>
        /// Updates product from warehouse
        /// </summary>
        /// <response code="204">Successfully updated Product</response>
        /// <response code="400">Product form malformed</response>
        /// <response code="401">Only for authorise user</response>
        /// <response code="404">Product with given id not found</response>
        /// <returns>Ok</returns>
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult> UpdateProduct(long productId, [FromBody] ProductForm productForm, [FromServices] UserInfo userInfo, CancellationToken token)
        {
            var validator = new ProductForm.Validator();
            var validationResult = await validator.ValidateAsync(productForm, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var product = await _context.Products
                .Include(p => p.QuantityChanges)
                .FirstOrDefaultAsync(p => p.Id == productId, token);
            
            if (product == null)
            {
                return NotFound();
            }
            
            var user = await _context.WmcUser
                .FirstAsync(wu => wu.Id == userInfo.UserId, token);
            
            product.UpdateProduct(Product.CreateNewProduct(productForm.ManufacturerName, productForm.ModelName, productForm.Price.Value, user));
            
            await _context.SaveChangesAsync(token);

            return NoContent();
        }

        /// <summary>
        /// Deletes product from warehouse
        /// </summary>
        /// <response code="200">Successfully deleted Product</response>
        /// <response code="401">Only for authorise user with manager role</response>
        /// <response code="403">User is not manager</response>
        /// <response code="404">Product with given id not found</response>
        /// <returns>Ok</returns>
        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = Policies.ManagerOnly)]
        public async Task<ActionResult> DeleteProduct(long productId, CancellationToken token)
        {
            var productToDelete = await _context.Products
                .Include(p => p.QuantityChanges)
                .FirstOrDefaultAsync(p => p.Id == productId, token);

            if (productToDelete == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync(token);

            return Ok();
        }

        /// <summary>
        /// Changes quantity of product in warehouse
        /// </summary>
        /// <response code="200">Successfully deleted Product</response>
        /// <response code="400">Quantity change can't change sum of product quantity to negative</response>
        /// <response code="401">Only for authorise user with manager role</response>
        /// <response code="404">Product with given id not found</response>
        /// <returns>Ok</returns>
        [HttpPut("{productId}/{quantityChange}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult> ChangeQuantity(long productId, long quantityChange, [FromServices] UserInfo userInfo, CancellationToken token)
        {
            var product = await _context.Products
                .Include(p => p.QuantityChanges)
                .FirstOrDefaultAsync(p => p.Id == productId, token);
            
            if (product == null)
            {
                return NotFound();
            }

            if (product.QuantityChanges.Sum(q => q.Quantity) + quantityChange < 0)
            {
                return BadRequest($"Result quantity can not be less than 0");
            }
            
            var user = await _context.WmcUser
                .FirstAsync(wu => wu.Id == userInfo.UserId, token);
            
            product.QuantityChanges.Add(ProductQuantityChange.CreateQuantityChange(quantityChange, user));
            await _context.SaveChangesAsync(token);
            
            return NoContent();
        }
    }
}