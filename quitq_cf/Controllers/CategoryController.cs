using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.Repository;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories.");
                var categories = await _categoryService.GetAllCategoriesAsync();
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NoContent(); 
                }

                _logger.LogInformation("Successfully fetched all categories.");
                return Ok(categories); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching categories: {ex.Message}");
                return StatusCode(500, "Error occurred while fetching categories"); 
            }
        }

        [HttpGet("subcategories/{categoryId}")]
        public async Task<IActionResult> GetSubcategories(int categoryId)
        {
            try
            {
                _logger.LogInformation($"Fetching subcategories for category ID: {categoryId}");
                var subcategories = await _categoryService.GetSubcategoriesByCategoryIdAsync(categoryId);
                if (subcategories == null || !subcategories.Any())
                {
                    _logger.LogWarning($"No subcategories found for category ID: {categoryId}");
                    return NoContent();
                }

                _logger.LogInformation($"Successfully fetched subcategories for category ID: {categoryId}");
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching subcategories for category {categoryId}: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error occurred while fetching subcategories for category");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] string categoryName)
        {
            try
            {
                _logger.LogInformation($"Attempting to create a new category with name: {categoryName}");
                var response = await _categoryService.CreateCategoryAsync(categoryName);

                if (response.Status is "Success")
                {
                    _logger.LogInformation($"Category '{categoryName}' created successfully.");
                    return CreatedAtAction(nameof(GetAllCategories), new { categoryName }, response);
                }

                _logger.LogWarning($"Category creation failed: {response.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating category: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error occurred while creating category");
            }
        }
        [HttpPost("{categoryId}/subcategory")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> CreateSubcategory(int categoryId, [FromBody] string subcategoryName)
        {
            try
            {
                _logger.LogInformation($"Attempting to create a new subcategory '{subcategoryName}' for category ID: {categoryId}");
                var response = await _categoryService.CreateSubcategoryAsync(categoryId, subcategoryName);

                if (response.Status is "Success")
                {
                    _logger.LogInformation($"Subcategory '{subcategoryName}' created successfully for category ID: {categoryId}");
                    return CreatedAtAction(nameof(GetSubcategories), new { categoryId }, response);
                }

                _logger.LogWarning($"Subcategory creation failed for category ID {categoryId}: {response.Message}");
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating subcategory for category ID {categoryId}: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error occurred while creating subcategory for given category ID"); 
            }
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] string categoryName)
        {
            try
            {
                _logger.LogInformation($"Attempting to update category with ID: {categoryId} to new name: {categoryName}");
                var response = await _categoryService.UpdateCategoryAsync(categoryId, categoryName);

                if (response.Status is "Success")
                {
                    _logger.LogInformation($"Category ID: {categoryId} updated successfully to '{categoryName}'");
                    return Ok(response); 
                }

                _logger.LogWarning($"Category update failed for ID {categoryId}: {response.Message}");
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating category ID {categoryId}: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error occurred while updating category");
            }
        }
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete category with ID: {categoryId}");
                var response = await _categoryService.DeleteCategoryAsync(categoryId);

                if (response.Status is "Success")
                {
                    _logger.LogInformation($"Category ID: {categoryId} deleted successfully.");
                    return Ok(response);
                }

                _logger.LogWarning($"Category deletion failed for ID {categoryId}: {response.Message}");
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while deleting category ID {categoryId}: {ex.Message}");
                return StatusCode(500, "Internal server error :  Error occurred while deleting category");
            }
        }
    }
}
