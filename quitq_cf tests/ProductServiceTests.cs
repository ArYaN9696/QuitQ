using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.DTO;
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
    public class ProductServiceTests
    {
        private ApplicationDbContext _context;
        private ProductService _productService;

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

            _productService = new ProductService(_context, mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { ProductId = 1, ProductName = "Product1", Price = 100 },
                new Product { ProductId = 2, ProductName = "Product2", Price = 200 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetProductByIdAsync_ReturnsProduct()
        {
            // Arrange
            var product = new Product { ProductId = 1, ProductName = "Product1", Price = 100 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Product1", result.ProductName);
        }

        [Test]
        public async Task CreateProductAsync_AddsProduct()
        {
            // Arrange
            var newProduct = new CreateProductDTO
            {
                ProductName = "New Product",
                Description = "Description",
                Price = 300,
                Stock = 10,
                SubcategoryId = 1
            };

            // Act
            var result = await _productService.CreateProductAsync("seller1", newProduct);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(1, _context.Products.Count());
        }


        [Test]
        public async Task SearchProductsAsync_ReturnsFilteredProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { ProductId = 1, ProductName = "ProductA", Price = 50 },
                new Product { ProductId = 2, ProductName = "ProductB", Price = 150 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.SearchProductsAsync("Product", 0, 100);

            // Assert
            Assert.AreEqual(1, result.Count());
        }
    }
}
