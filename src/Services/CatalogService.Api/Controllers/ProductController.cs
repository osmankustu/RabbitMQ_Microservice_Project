using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Core.Application.ViewModels;
using ProductService.Api.Core.Domain;
using ProductService.Api.Infrastructure.Context;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProductService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _productContext;

        public ProductController(ProductContext productContext)
        {
            _productContext = productContext;
            _productContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedProductsViewModel<Product>),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ProductAsync([FromQuery] int pageSize = 10,[FromQuery] int pageIndex = 0, string? ids=null)
        {
            
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdAsync(ids);
                if (!items.Any())
                {
                    return BadRequest("ids value not invaild. Must be comma-seperated list of numbers");
                }

                return Ok(items);
            }

            var totalItems = await _productContext.Products
                .LongCountAsync();
            
            var itemsOnPaged = await _productContext.Products
                .Include(p=>p.ProductBrand)
                .OrderBy(c=>c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();


            var model = new PaginatedProductsViewModel<Product>(pageIndex, pageSize, totalItems, itemsOnPaged);

            return Ok(model);
        }

        private async Task<List<Product>> GetItemsByIdAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid=>nid.Ok))
            {
                return new List<Product>();
            }

            var idToSelect = numIds
                .Select(id => id.Value);

            var items = await _productContext.Products
                .Include(p=>p.ProductCategory)
                .Include(p=>p.ProductBrand)
                .Where(ci => idToSelect.Contains(ci.Id))
                .ToListAsync();

            return items;
        }

    }
}
