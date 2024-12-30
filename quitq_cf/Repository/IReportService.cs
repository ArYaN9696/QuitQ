namespace quitq_cf.Repository
{
    public interface IReportService
    {
        Task<dynamic> GenerateSalesReportAsync(string sellerId, DateTime startDate, DateTime endDate);
        Task<dynamic> GenerateUserActivityReportAsync(DateTime startDate, DateTime endDate);
        Task<dynamic> GenerateInventoryReportAsync();
        Task<IEnumerable<RevenueReportDTO>> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate);
    }
}
