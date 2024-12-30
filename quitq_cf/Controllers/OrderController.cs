using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Repository;
using System.Security.Claims;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                _logger.LogWarning("CreateOrder failed: User is not authenticated.");
                return Unauthorized(new Response { Status = "Failed", Message = "User is not authenticated" });
            }

            try
            {
                var response = await _orderService.CreateOrderAsync(userId, orderDto);

                if (response != null)
                {
                    _logger.LogInformation("Order created successfully for User: {UserId}", userId);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("CreateOrder failed: No response from service.");
                    return BadRequest(new Response { Status = "Failed", Message = "Order creation failed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for User: {UserId}", userId);
                return StatusCode(500, new Response { Status = "Failed", Message = "An error occurred while creating the order" });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = "Customer, Seller, Admin")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var response = await _orderService.GetOrderByIdAsync(orderId);

                if (response != null)
                {
                    _logger.LogInformation("Order retrieved successfully: {OrderId}", orderId);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Order not found: {OrderId}", orderId);
                    return NotFound(new Response { Status = "Failed", Message = "Order not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order: {OrderId}", orderId);
                return StatusCode(500, new Response { Status = "Failed", Message = "An error occurred while retrieving the order" });
            }
        }

        [HttpGet("user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new Response { Status = "Failed", Message = "User ID not found in token" });
            }

            try
            {
                var response = await _orderService.GetUserOrdersAsync(userId);

                if (response != null)
                {
                    return Ok(response);
                }
                else
                {
                    return NotFound(new { Message = "No orders found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user: {UserId}", userId);
                return StatusCode(500, new { Message = "An error occurred while retrieving orders." });
            }
        }


        [HttpGet("seller/{sellerId}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> GetSellerOrders(string sellerId)
        {
            try
            {
                var response = await _orderService.GetSellerOrdersAsync(sellerId);

                if (response != null)
                {
                    _logger.LogInformation("Orders retrieved for Seller: {SellerId}", sellerId);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("No orders found for Seller: {SellerId}", sellerId);
                    return NotFound(new Response { Status = "Failed", Message = "No orders found for this seller" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for Seller: {SellerId}", sellerId);
                return StatusCode(500, new Response { Status = "Failed", Message = "An error occurred while retrieving seller orders" });
            }
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] int statusId)
        {
            try
            {
                var response = await _orderService.UpdateOrderStatusAsync(orderId, statusId);

                if (response.Status == "Success")
                {
                    _logger.LogInformation("Order status updated successfully: {OrderId}", orderId);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Failed to update status for Order: {OrderId}", orderId);
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status: {OrderId}", orderId);
                return StatusCode(500, new Response { Status = "Failed", Message = "An error occurred while updating the order status" });
            }
        }

        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Customer, Seller")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var response = await _orderService.CancelOrderAsync(orderId);

                if (response.Status == "Success")
                {
                    _logger.LogInformation("Order cancelled successfully: {OrderId}", orderId);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Failed to cancel order: {OrderId}", orderId);
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", orderId);
                return StatusCode(500, new Response { Status = "Failed", Message = "An error occurred while cancelling the order" });
            }
        }
    }
}
