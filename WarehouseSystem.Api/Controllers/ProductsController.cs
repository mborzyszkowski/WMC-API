using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Core.Entity;
using WarehouseSystem.Query;
using WarehouseSystem.Repository;

namespace WarehouseSystem.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly WarehouseDbContext _context;

        public ProductsController(WarehouseDbContext context) => 
            _context = context;

        /// <summary>
        /// Get all products from warehouse
        /// </summary>
        /// <response code="200">Successfully retrieved Products</response>
        /// <returns>All products info in warehouse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<ProductResult>>> SearchAllProducts(CancellationToken token) =>
            await _context.Products
                .Include(p => p.QuantityChanges)
                .SelectMany(
                    p => p.QuantityChanges,
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
                .ToListAsync(cancellationToken: token);

        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResult>> GetProduct(long productId, CancellationToken token) =>
            await _context.Products
                .Include(p => p.QuantityChanges)
                .SelectMany(
                    p => p.QuantityChanges,
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
                .FirstAsync(p => p.Id == productId, token);
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProduct([FromBody] ProductForm productForm, CancellationToken token)
        {
            var validator = new ProductForm.Validator();
            var validationResult = await validator.ValidateAsync(productForm, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }
            
            var newProduct = Product.CreateNewProduct(productForm.ManufacturerName, productForm.ModelName, productForm.Price.Value);

            await _context.Products.AddAsync(newProduct, token);
            await _context.SaveChangesAsync(token);

            return Ok();
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProduct(long productId, [FromBody] ProductForm productForm, CancellationToken token)
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
            
            //Rewrite productForm to product by AutoMaper
            product.UpdateProduct(Product.CreateNewProduct(productForm.ManufacturerName, productForm.ModelName, productForm.Price.Value));
            
            await _context.SaveChangesAsync(token);

            return Ok();
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPut("{productId}/{quantityChange}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeQuantity(long productId, long quantityChange, CancellationToken token)
        {
            var product = await _context.Products
                .Include(p => p.QuantityChanges)
                .FirstOrDefaultAsync(p => p.Id == productId, token);
            
            if (product == null)
            {
                return NotFound();
            }

            if (product.QuantityChanges.Sum(q => q.Quantity) - quantityChange < 0)
            {
                //TODO: more info???
                return BadRequest();
            }
            
            product.QuantityChanges.Add(ProductQuantityChange.CreateQuantityChange(quantityChange));
            await _context.SaveChangesAsync(token);
            
            return Ok();
        }
    }
}