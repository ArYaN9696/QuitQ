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

    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                _logger.LogInformation($"Fetching cart items for user ID: {userId}");
                var cartItems = await _cartService.GetCartItemsAsync(userId);

                if (cartItems == null || !cartItems.Any())
                {
                    _logger.LogWarning($"No cart items found for user ID: {userId}");
                    return NoContent();
                }

                _logger.LogInformation($"Successfully fetched cart items for user ID: {userId}");
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching cart items for user ID: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching cart items.");
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDTO item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {

                _logger.LogInformation($"Attempting to add product {item.ProductId} to cart for user ID: {userId}");
                var response = await _cartService.AddToCartAsync(userId, item);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Product {item.ProductId} added to cart successfully for user ID: {userId}");
                    return CreatedAtAction(nameof(GetCartItems), new { userId }, response);
                }

                _logger.LogWarning($"Failed to add product to cart for user ID {userId}: {response.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while adding product to cart for user ID {userId}: {ex.Message}");
                return StatusCode(500, "Error occurred while adding product to cart");
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemDTO item )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                _logger.LogInformation($"Attempting to update cart item {item.ProductId} for user ID: {userId}");
                var response = await _cartService.UpdateCartItemAsync(userId, item);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Cart item {item.ProductId} updated successfully for user ID: {userId}");
                    return Ok(response);
                }

                _logger.LogWarning($"Failed to update cart item for user ID {userId}: {response.Message}");
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating cart item for user ID {userId}: {ex.Message}");
                return StatusCode(500, "Error occurred while updating cart item");
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                _logger.LogInformation($"Attempting to remove product {productId} from cart for user ID: {userId}");
                var response = await _cartService.RemoveFromCartAsync(userId, productId);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Product {productId} removed from cart successfully for user ID: {userId}");
                    return Ok(response);
                }

                _logger.LogWarning($"Failed to remove product from cart for user ID {userId}: {response.Message}");
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing product from cart for user ID {userId}: {ex.Message}");
                return StatusCode(500, "Error occurred while removing product from cart");
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                _logger.LogInformation($"Attempting to clear cart for user ID: {userId}");
                var response = await _cartService.ClearCartAsync(userId);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Cart cleared successfully for user ID: {userId}");
                    return Ok(response);
                }

                _logger.LogWarning($"Failed to clear cart for user ID {userId}: {response.Message}");
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while clearing cart for user ID {userId}: {ex.Message}");
                return StatusCode(500, "Error occurred while clearing cart");
            }
        }
    }
}
