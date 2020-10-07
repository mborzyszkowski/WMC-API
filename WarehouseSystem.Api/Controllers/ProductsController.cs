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
        //TODO: rewrite to return quantity, not changes
        public async Task<ActionResult<IReadOnlyCollection<ProductResult>>> SearchAllProducts(CancellationToken token) =>
            throw new NotImplementedException();
        
        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //TODO: rewrite to return quantity, not changes
        public async Task<ActionResult<ProductResult>> GetProduct(long productId, CancellationToken token) =>
            throw new NotImplementedException();
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddProduct([FromBody] ProductForm productForm, CancellationToken token) =>
            throw new NotImplementedException();
        
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateProduct(long productId, [FromBody] ProductForm productForm, CancellationToken token) =>
            throw new NotImplementedException();
        
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteProduct(long productId, CancellationToken token) => 
            throw new NotImplementedException();
        
        [HttpPut("quantity/{productId}/{quantityChange}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ChangeQuantity(long productId, long quantityChange, CancellationToken token) => 
            throw new NotImplementedException();
    }
}