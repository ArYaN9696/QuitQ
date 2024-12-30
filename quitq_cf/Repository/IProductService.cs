using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDTO>> GetProductsBySellerIdAsync(string sellerId);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<Response> CreateProductAsync(string sellerId, CreateProductDTO product);
        Task<Response> UpdateProductAsync(int productId, string sellerId, UpdateProductDTO product);
        Task<Response> DeleteProductAsync(int productId, string sellerId);
        Task<Response> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDTO>> SearchProductsAsync(string? query, decimal? minPrice, decimal? maxPrice);

        Task DeleteProductAsync(int productId);
    }
}
