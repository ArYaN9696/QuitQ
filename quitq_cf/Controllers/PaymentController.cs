using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.DTO;
using quitq_cf.Models;
using quitq_cf.Repository;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDTO paymentDto)
        {
            try
            {
                _logger.LogInformation($"Attempting to process payment for order ID: {paymentDto.OrderId}");
                var response = await _paymentService.ProcessPaymentAsync(paymentDto.OrderId, paymentDto.PaymentMethod, paymentDto.Amount);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Payment for order ID {paymentDto.OrderId} processed successfully.");
                    return Ok(response);
                }

                _logger.LogWarning($"Payment processing failed for order ID {paymentDto.OrderId}: {response.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while processing payment for order ID {paymentDto.OrderId}: {ex.Message}");
                return StatusCode(500, "Error occurred while processing payment");
            }
        }

        [Authorize(Roles = " Seller, Admin")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePayment([FromQuery] string transactionId)
        {
            try
            {
                _logger.LogInformation($"Validating payment with transaction ID: {transactionId}");
                var response = await _paymentService.ValidatePaymentAsync(transactionId);

                if (response.Status == "Success")
                {
                    _logger.LogInformation($"Payment with transaction ID {transactionId} validated successfully.");
                    return Ok(response);
                }

                _logger.LogWarning($"Payment validation failed for transaction ID {transactionId}: {response.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while validating payment with transaction ID {transactionId}: {ex.Message}");
                return StatusCode(500, "Error occurred while validating payment");
            }
        }
        [Authorize(Roles = "Seller, Admin")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            try
            {
                _logger.LogInformation($"Fetching payments for order ID: {orderId}");
                var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                if (payments == null || !payments.Any())
                {
                    _logger.LogWarning($"No payments found for order ID: {orderId}");
                    return NoContent();
                }

                _logger.LogInformation($"Successfully fetched payments for order ID: {orderId}");
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching payments for order ID {orderId}: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching payments");
            }
        }
    }
}
