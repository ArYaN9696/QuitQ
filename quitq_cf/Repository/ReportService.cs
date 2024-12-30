using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace quitq_cf.Repository
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _appDBContext;

        public ReportService(ApplicationDbContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<dynamic> GenerateSalesReportAsync(string sellerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var salesReport = await _appDBContext.Payments
                    .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate) 
                    .Join(_appDBContext.Orders,
                        p => p.OrderId, 
                        o => o.OrderId,
                        (p, o) => new { o.OrderId, p.PaymentDate }) 
                    .Join(_appDBContext.OrderDetails,
                        o => o.OrderId, 
                        od => od.OrderId,
                        (o, od) => new
                        {
                            o.OrderId,
                            o.PaymentDate,
                            od.ProductId,
                            od.Quantity,
                            od.Price,
                            TotalSales = od.Quantity * od.Price
                        })
                    .Join(_appDBContext.Products,
                        od => od.ProductId, 
                        p => p.ProductId,
                        (od, p) => new
                        {
                            od.OrderId,
                            od.PaymentDate,
                            od.Quantity,
                            od.Price,
                            od.TotalSales,
                            p.SellerId 
                        })
                    .Where(x => x.SellerId == sellerId) 
                    .GroupBy(x => new { x.OrderId, x.PaymentDate })
                    .Select(g => new
                    {
                        g.Key.OrderId,
                        g.Key.PaymentDate,
                        TotalSales = g.Sum(x => x.TotalSales) 
                    })
                    .ToListAsync();

                return salesReport;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { Status = "Failure", Message = "Error generating sales report, please try again later." };
            }
        }


        public async Task<dynamic> GenerateUserActivityReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var userActivityReport = await _appDBContext.Payments
                    .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate) 
                    .Join(_appDBContext.Orders,
                        p => p.OrderId, 
                        o => o.OrderId,
                        (p, o) => new
                        {
                            o.UserId, 
                            p.PaymentDate,
                            ActivityType = "Payment",
                            p.Amount, 
                            o.ShippingAddress 
                        })
                    .ToListAsync(); 

                return userActivityReport;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { Status = "Failure", Message = "Error generating user activity report, please try again later." };
            }
        }


        public async Task<dynamic> GenerateInventoryReportAsync()
        {
            try
            {
                var inventoryReport = await _appDBContext.Products
                    .Join(_appDBContext.StockInfos,
                        p => p.ProductId,
                        si => si.ProductId,
                        (p, si) => new
                        {
                            p.ProductId,
                            p.ProductName,
                            p.Description,
                            p.Price,
                            si.Quantity
                        })
                    .ToListAsync();

                return inventoryReport;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { Status = "Failure", Message = "Error generating inventory report, please try again later." };
            }
        }


        public async Task<IEnumerable<RevenueReportDTO>> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var revenueReport = await _appDBContext
                    .Set<RevenueReportDTO>()
                    .FromSqlRaw(@"SELECT CONVERT(date, payment_date) AS PaymentDate, SUM(Amount) AS TotalRevenue
                      FROM Payments
                      WHERE payment_date >= {0} AND payment_date <= {1}
                      GROUP BY CONVERT(date, payment_date);" ,startDate, endDate)
                    .ToListAsync();

                foreach (var report in revenueReport)
                {
                    Console.WriteLine($"Date: {report.Date}, TotalRevenue: {report.TotalRevenue}");
                }

                return revenueReport;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

        }




    }
}
