using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _appDBContext;

        public PaymentService(ApplicationDbContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<Response> ProcessPaymentAsync(int orderId, string paymentMethod, decimal amount)
        {
            try
            {
                var order = await _appDBContext.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new Response { Status = "Failure", Message = "Order not found." };
                }

                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentStatus = "Completed"  
                };

                _appDBContext.Payments.Add(payment);
                await _appDBContext.SaveChangesAsync();

                return new Response { Status = "Success", Message = "Payment processed successfully." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessPaymentAsync: {ex.Message}");

                // In case of failure, you can return a status with failure
                return new Response { Status = "Failure", Message = "An error occurred while processing the payment." };
            }
        }


        public async Task<Response> ValidatePaymentAsync(string transactionId)
        {
            try
            {
                var payment = await _appDBContext.Payments
                    .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

                if (payment == null)
                {
                    return new Response { Status = "Failure", Message = "Payment not found." };
                }

                return new Response { Status = "Success", Message = "Payment is valid." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ValidatePaymentAsync: {ex.Message}");
                return new Response { Status = "Failure", Message = "An error occurred while validating the payment." };
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            try
            {
                var payments = await _appDBContext.Payments
                    .Where(p => p.OrderId == orderId)
                    .ToListAsync();

                return payments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPaymentsByOrderIdAsync: {ex.Message}");
                return Enumerable.Empty<Payment>();
            }
        }
    }
}
