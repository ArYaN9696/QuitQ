using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.Repository;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }
        [Authorize(Roles = "Seller")]
        [HttpGet("sales/{sellerId}")]
        public async Task<IActionResult> GetSalesReport(string sellerId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Fetching sales report for seller ID: {sellerId} between {startDate} and {endDate}");
                var salesReport = await _reportService.GenerateSalesReportAsync(sellerId, startDate, endDate);
                if (salesReport == null)
                {
                    _logger.LogWarning($"No sales data found for seller ID: {sellerId}");
                    return NoContent();
                }

                _logger.LogInformation($"Successfully fetched sales report for seller ID: {sellerId}");
                return Ok(salesReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching sales report for seller ID {sellerId}: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching sales report");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("user-activity")]
        public async Task<IActionResult> GetUserActivityReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Fetching user activity report between {startDate} and {endDate}");
                var userActivityReport = await _reportService.GenerateUserActivityReportAsync(startDate, endDate);
                if (userActivityReport == null)
                {
                    _logger.LogWarning("No user activity data found.");
                    return NoContent();
                }

                _logger.LogInformation("Successfully fetched user activity report.");
                return Ok(userActivityReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching user activity report: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching user activity report");
            }
        }
        [Authorize(Roles = "Seller, Admin")]
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryReport()
        {
            try
            {
                _logger.LogInformation("Fetching inventory report.");
                var inventoryReport = await _reportService.GenerateInventoryReportAsync();
                if (inventoryReport == null)
                {
                    _logger.LogWarning("No inventory data found.");
                    return NoContent();
                }

                _logger.LogInformation("Successfully fetched inventory report.");
                return Ok(inventoryReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching inventory report: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching inventory report");
            }
        }
        [Authorize(Roles = "Seller, Admin")]
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Fetching revenue report between {startDate} and {endDate}");
                var revenueReport = await _reportService.GenerateRevenueReportAsync(startDate, endDate);
                if (revenueReport == null || !revenueReport.Any())
                {
                    _logger.LogWarning("No revenue data found.");
                    return NoContent();
                }

                _logger.LogInformation("Successfully fetched revenue report.");
                return Ok(revenueReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching revenue report: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching revenue report");
            }
        }
    }
}
