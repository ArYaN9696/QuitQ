using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface IPaymentService
    {
        Task<Response> ProcessPaymentAsync(int orderId, string paymentMethod, decimal amount);
        Task<Response> ValidatePaymentAsync(string transactionId);
        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);
    }
}
