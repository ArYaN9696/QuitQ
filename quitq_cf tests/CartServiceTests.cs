using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using quitq_cf.Data;
using quitq_cf.Models;
using quitq_cf.Repository;
using quitq_cf.DTO;
using System.Linq;
using System.Threading.Tasks;

namespace quitq_cf_tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private ApplicationDbContext _context;
        private CartService _cartService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _cartService = new CartService(_context);

            // Seed initial data
            _context.Carts.Add(new Cart { UserId = "testUser", ProductId = 1, Quantity = 2 });
            _context.Carts.Add(new Cart { UserId = "testUser", ProductId = 2, Quantity = 1 });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCartItemsAsync_ReturnsCartItems()
        {
            // Arrange
            var product1 = new Product { ProductId = 1, ProductName = "Product 1", Price = 100 };
            var product2 = new Product { ProductId = 2, ProductName = "Product 2", Price = 200 };

            _context.Products.Add(product1);
            _context.Products.Add(product2);

            _context.Carts.Add(new Cart { UserId = "testUser", ProductId = 1, Quantity = 2 });
            _context.Carts.Add(new Cart { UserId = "testUser", ProductId = 2, Quantity = 1 });

            await _context.SaveChangesAsync();

            // Act
            var result = await _cartService.GetCartItemsAsync("testUser");

            // Assert
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.Any(c => c.ProductId == 1 && c.Quantity == 2));
            Assert.IsTrue(result.Any(c => c.ProductId == 2 && c.Quantity == 1));
        }


        [Test]
        public async Task AddToCartAsync_AddsNewItemToCart()
        {
            // Arrange
            var newItem = new CartItemDTO { ProductId = 3, Quantity = 5 };

            // Act
            var result = await _cartService.AddToCartAsync("testUser", newItem);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(_context.Carts.Any(c => c.UserId == "testUser" && c.ProductId == 3));
        }

        [Test]
        public async Task AddToCartAsync_IncrementsQuantityIfItemExists()
        {
            // Arrange
            var existingItem = new CartItemDTO { ProductId = 1, Quantity = 3 };

            // Act
            var result = await _cartService.AddToCartAsync("testUser", existingItem);

            // Assert
            Assert.AreEqual("Success", result.Status);
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == "testUser" && c.ProductId == 1);
            Assert.AreEqual(5, cartItem.Quantity); // 2(existing) + 3(new)
        }

        [Test]
        public async Task UpdateCartItemAsync_UpdatesQuantity()
        {
            // Arrange
            var updatedItem = new CartItemDTO { ProductId = 1, Quantity = 10 };

            // Act
            var result = await _cartService.UpdateCartItemAsync("testUser", updatedItem);

            // Assert
            Assert.AreEqual("Success", result.Status);
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == "testUser" && c.ProductId == 1);
            Assert.AreEqual(10, cartItem.Quantity);
        }

        [Test]
        public async Task UpdateCartItemAsync_ReturnsFailureIfItemNotFound()
        {
            // Arrange
            var nonExistentItem = new CartItemDTO { ProductId = 999, Quantity = 5 };

            // Act
            var result = await _cartService.UpdateCartItemAsync("testUser", nonExistentItem);

            // Assert
            Assert.AreEqual("Failure", result.Status);
        }

        [Test]
        public async Task RemoveFromCartAsync_RemovesItemFromCart()
        {
            // Act
            var result = await _cartService.RemoveFromCartAsync("testUser", 1);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.IsFalse(_context.Carts.Any(c => c.UserId == "testUser" && c.ProductId == 1));
        }

        [Test]
        public async Task RemoveFromCartAsync_ReturnsFailureIfItemNotFound()
        {
            // Act
            var result = await _cartService.RemoveFromCartAsync("testUser", 999);

            // Assert
            Assert.AreEqual("Failure", result.Status);
        }

        [Test]
        public async Task ClearCartAsync_ClearsAllItems()
        {
            // Act
            var result = await _cartService.ClearCartAsync("testUser");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.IsFalse(_context.Carts.Any(c => c.UserId == "testUser"));
        }

        [Test]
        public async Task ClearCartAsync_ReturnsSuccessWhenCartIsEmpty()
        {
            // Arrange
            await _cartService.ClearCartAsync("testUser");

            // Act
            var result = await _cartService.ClearCartAsync("testUser");

            // Assert
            Assert.AreEqual("Success", result.Status);
        }
    }
}
