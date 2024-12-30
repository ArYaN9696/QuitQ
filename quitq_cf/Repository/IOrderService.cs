using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface IOrderService
    {
        Task<DTO.OrderDTO> CreateOrderAsync(string userId, CreateOrderDTO orderDto);
        Task<DTO.OrderDTO> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<IEnumerable<DTO.OrderDTO>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<DTO.OrderDTO>> GetSellerOrdersAsync(string sellerId);
        Task<Response> UpdateOrderStatusAsync(int orderId, int statusId);
        Task<Response> CancelOrderAsync(int orderId);
    }
}
