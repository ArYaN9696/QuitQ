using quitq_cf.Data;
using quitq_cf.Models;
using quitq_cf.DTO;
namespace quitq_cf.Repository
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string userId);
        Task<Response> AddToCartAsync(string userId, CartItemDTO item);
        Task<Response> UpdateCartItemAsync(string userId, CartItemDTO item);
        Task<Response> RemoveFromCartAsync(string userId, int productId);
        Task<Response> ClearCartAsync(string userId);
    }
}
