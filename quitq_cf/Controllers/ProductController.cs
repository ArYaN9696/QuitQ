using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.DTO;
using quitq_cf.Repository;
using System.Security.Claims;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching products: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error fetching products");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { Message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching product by ID: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error fetching product");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO productDto)
        {
            try
            {
                var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized(new { Message = "Seller ID is required" });
                }

                var response = await _productService.CreateProductAsync(sellerId, productDto);
                if (response.Status is "Success")
                {
                    return CreatedAtAction(nameof(GetProductById), response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating product: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error creating product");
            }
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductDTO productDto)
        {
            try
            {
                var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized(new { Message = "Seller ID is required" });
                }

                var response = await _productService.UpdateProductAsync(productId, sellerId, productDto);
                if (response.Status is "Success")
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error updating product");
            }
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized(new { Message = "Seller ID is required" });
                }

                var response = await _productService.DeleteProductAsync(productId, sellerId);
                if (response.Status is "Success")
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting product: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error deleting product");
            }
        }

        [HttpPut("{productId}/stock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] int quantity)
        {
            try
            {
                var response = await _productService.UpdateStockAsync(productId, quantity);
                if (response.Status is "Success")
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating stock: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error updating stock");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? query, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(query, minPrice, maxPrice);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while searching for products.", Error = ex.Message });
            }
        }
    }
}
