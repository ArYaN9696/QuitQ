using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<SubCategory>> GetSubcategoriesByCategoryIdAsync(int categoryId);
        Task<Response> CreateCategoryAsync(string categoryName);
        Task<Response> CreateSubcategoryAsync(int categoryId, string subcategoryName);
        Task<Response> UpdateCategoryAsync(int categoryId, string categoryName);
        Task<Response> DeleteCategoryAsync(int categoryId);
    }
}
