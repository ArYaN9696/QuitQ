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
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace quitq_cf_tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ApplicationDbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Suppress the transaction warning
                .Options;

            _context = new ApplicationDbContext(options);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            var mapper = mapperConfiguration.CreateMapper();

            _orderService = new OrderService(_context, mapper);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        //[Test]
        //public async Task CreateOrderAsync_AddsOrder()
        //{
        //    // Arrange
        //    var cartItems = new List<Cart>
        //    {
        //        new Cart { ProductId = 1,Quantity = 2, Product = new Product { Price = 100,ProductName="sample" } }
        //    };
        //    _context.Carts.AddRange(cartItems);
        //    await _context.SaveChangesAsync();

        //    var newOrder = new CreateOrderDTO
        //    {
        //        ShippingAddress = "Test Address",
        //        PaymentMethod = "Credit Card"
        //    };

        //    // Act
        //    var result = await _orderService.CreateOrderAsync("user1", newOrder);

        //    // Assert
        //    Assert.AreEqual("Test Address", result.ShippingAddress);
        //    Assert.AreEqual(200, result.TotalAmount);
        //}

        [Test]
        public async Task GetUserOrdersAsync_ReturnsUserOrders()
        {
            // Arrange
            _context.Orders.Add(new Order { OrderId = 1, UserId = "user1", TotalAmount = 100,PaymentMethod="UPI",ShippingAddress="xyz" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.GetUserOrdersAsync("user1");

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task CancelOrderAsync_CancelsOrder()
        {
            // Arrange
            var order = new Order { OrderId = 1, StatusId = 1, PaymentMethod = "UPI", ShippingAddress = "xyz" };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.CancelOrderAsync(1);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(5, order.StatusId); // Assuming 5 is 'Cancelled'
        }
    }
}
