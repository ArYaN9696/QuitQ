using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;
using quitq_cf.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using quitq_cf.Mapping;

namespace quitq_cf_tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private ApplicationDbContext _context;
        private CategoryService _categoryService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Create a mock for IMapper
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); 
            });

            var mapper = mapperConfiguration.CreateMapper();

            _categoryService = new CategoryService(_context, mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category { CategoryId = 1, CategoryName = "Category 1" },
                new Category { CategoryId = 2, CategoryName = "Category 2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(c => c.CategoryName == "Category 1"));
        }

        [Test]
        public async Task GetSubcategoriesByCategoryIdAsync_ReturnsSubcategories()
        {
            // Arrange
            _context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Category 1" });
            _context.SubCategories.AddRange(
                new SubCategory { SubcategoryId = 1, SubcategoryName = "SubCategory 1", CategoryId = 1 },
                new SubCategory { SubcategoryId = 2, SubcategoryName = "SubCategory 2", CategoryId = 1 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.GetSubcategoriesByCategoryIdAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(sc => sc.SubcategoryName == "SubCategory 1"));
        }

        [Test]
        public async Task CreateCategoryAsync_AddsNewCategory()
        {
            // Act
            var result = await _categoryService.CreateCategoryAsync("New Category");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(1, _context.Categories.Count());
            Assert.IsTrue(_context.Categories.Any(c => c.CategoryName == "New Category"));
        }

        [Test]
        public async Task CreateCategoryAsync_ReturnsFailureIfExists()
        {
            // Arrange
            _context.Categories.Add(new Category { CategoryName = "Existing Category" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.CreateCategoryAsync("Existing Category");

            // Assert
            Assert.AreEqual("Failed", result.Status);
        }

        [Test]
        public async Task UpdateCategoryAsync_UpdatesCategoryName()
        {
            // Arrange
            var category = new Category { CategoryId = 1, CategoryName = "Old Name" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.UpdateCategoryAsync(1, "New Name");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("New Name", _context.Categories.First(c => c.CategoryId == 1).CategoryName);
        }

        [Test]
        public async Task DeleteCategoryAsync_DeletesCategory()
        {
            // Arrange
            var category = new Category { CategoryId = 1, CategoryName = "Category to Delete" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.DeleteCategoryAsync(1);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(0, _context.Categories.Count());
        }
    }
}
