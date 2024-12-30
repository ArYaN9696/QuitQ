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
    public class PaymentServiceTests
    {
        private ApplicationDbContext _context;
        private IPaymentService _paymentService;

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

            // Assuming PaymentService implements IPaymentService
            _paymentService = new PaymentService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task ProcessPaymentAsync_ReturnsSuccess_WhenValidData()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                TotalAmount = 100.0M,
                ShippingAddress = "123 Test St",
                PaymentMethod = "CreditCard",
                StatusId = 1 // Assuming an order status exists
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentService.ProcessPaymentAsync(1, "CreditCard", 100.0M);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(_context.Payments.Any(p => p.OrderId == 1 && p.Amount == 100.0M && p.PaymentMethod == "CreditCard"));
        }

        [Test]
        public async Task ProcessPaymentAsync_ReturnsFailure_WhenOrderNotFound()
        {
            // Act
            var result = await _paymentService.ProcessPaymentAsync(999, "CreditCard", 100.0M);

            // Assert
            Assert.AreEqual("Failure", result.Status);
            Assert.AreEqual("Order not found.", result.Message);
        }

        [Test]
        public async Task ValidatePaymentAsync_ReturnsSuccess_WhenValidTransactionId()
        {
            // Arrange
            var payment = new Payment
            {
                PaymentId = "TX123",
                OrderId = 1,
                Amount = 100.0M,
                PaymentMethod = "CreditCard",
                PaymentDate = DateTime.UtcNow,
                TransactionId = "TX123",
                PaymentStatus = "Completed"
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentService.ValidatePaymentAsync("TX123");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Payment is valid.", result.Message);
        }

        [Test]
        public async Task ValidatePaymentAsync_ReturnsFailure_WhenInvalidTransactionId()
        {
            // Act
            var result = await _paymentService.ValidatePaymentAsync("InvalidTX");

            // Assert
            Assert.AreEqual("Failure", result.Status);
            Assert.AreEqual("Payment not found.", result.Message);
        }

        [Test]
        public async Task GetPaymentsByOrderIdAsync_ReturnsPayments()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                TotalAmount = 100.0M,
                ShippingAddress = "123 Test St",
                PaymentMethod = "CreditCard",
                StatusId = 1
            };
            _context.Orders.Add(order);
            _context.Payments.AddRange(
                new Payment { PaymentId = "TX1", TransactionId="tt1",OrderId = 1, Amount = 50.0M, PaymentMethod = "CreditCard", PaymentStatus = "Completed" },
                new Payment { PaymentId = "TX2", TransactionId="tt2", OrderId = 1, Amount = 50.0M, PaymentMethod = "PayPal", PaymentStatus = "Completed" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentService.GetPaymentsByOrderIdAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(p => p.PaymentMethod == "CreditCard"));
            Assert.IsTrue(result.Any(p => p.PaymentMethod == "PayPal"));
        }
    }
}
