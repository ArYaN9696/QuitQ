using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class CategoryService:ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        //private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .ToListAsync();
        }
        public async Task<IEnumerable<SubCategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            return await _context.SubCategories
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
        }
        public async Task<Response> CreateCategoryAsync(string categoryName)
        {
            try
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == categoryName);

                if (existingCategory != null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Category already exists"
                    };
                }

                var category = new Category
                {
                    CategoryName = categoryName
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Category created successfully"
                };
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error Occured while adding Category {categoryName}");
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error creating category: {ex.Message}"
                };
            }
        }
        public async Task<Response> CreateSubcategoryAsync(int categoryId, string subcategoryName)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Category not found"
                    };
                }

                var existingSubcategory = await _context.SubCategories
                    .FirstOrDefaultAsync(s => s.SubcategoryName == subcategoryName && s.CategoryId == categoryId);

                if (existingSubcategory != null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Subcategory already exists in this category"
                    };
                }

                var subcategory = new SubCategory
                {
                    CategoryId = categoryId,
                    SubcategoryName = subcategoryName
                };

                await _context.SubCategories.AddAsync(subcategory);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Subcategory created successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    Status = "Failed",
                    Message = $"Error creating subcategory: {ex.Message}"
                };
            }
        }
        public async Task<Response> UpdateCategoryAsync(int categoryId, string categoryName)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Category not found"
                    };
                }

                category.CategoryName = categoryName;
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Category updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error updating category: {ex.Message}"
                };
            }
        }

        public async Task<Response> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                {
                    return new Response
                    {
                        Status = "Failed",
                        Message = "Category not found"
                    };
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Category deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = $"Error deleting category: {ex.Message}"
                };
            }
        }
    }
}
