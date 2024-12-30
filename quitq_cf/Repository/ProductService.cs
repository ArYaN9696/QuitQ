using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class ProductService:IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Subcategory)
                .Include(p => p.Seller)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Subcategory)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsBySellerIdAsync(string sellerId)
        {
            var products = await _context.Products
                .Include(p => p.Subcategory)
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Subcategory)
                .Where(p => p.Subcategory.CategoryId == categoryId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<Response> CreateProductAsync(string sellerId, CreateProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                product.SellerId = sellerId;

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Product created successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error creating product: {ex.Message}"
                };
            }
        }

        public async Task<Response> UpdateProductAsync(int productId, string sellerId, UpdateProductDTO productDto)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.SellerId == sellerId);

                if (product == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Product not found or you're not authorized to update it"
                    };
                }

                _mapper.Map(productDto, product);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Product updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error updating product: {ex.Message}"
                };
            }
        }

        public async Task<Response> DeleteProductAsync(int productId, string sellerId)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.SellerId == sellerId);

                if (product == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Product not found or you're not authorized to delete it"
                    };
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Product deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error deleting product: {ex.Message}"
                };
            }
        }

        public async Task<Response> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Product not found"
                    };
                }

                product.Stock = quantity;
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Stock updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error updating stock: {ex.Message}"
                };
            }
        }

        public Task DeleteProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string? query, decimal? minPrice, decimal? maxPrice)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(query) || p.Description.Contains(query));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            var products = await productsQuery.ToListAsync();

            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
            });
        }
    }
}
