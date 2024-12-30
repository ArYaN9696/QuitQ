using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;
using quitq_cf.Repository;
using System;
using System.Threading.Tasks;
using Moq;

namespace quitq_cf_tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private ApplicationDbContext _context;
        private ICustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Assuming the authorization service is mocked or created here
            var mockAuthorizationService = new Mock<IAuthorisationService>();
            _customerService = new CustomerService(mockAuthorizationService.Object, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateCustomerAsync_ReturnsSuccess_WhenValidCustomer()
        {
            // Arrange
            var newCustomer = new Customer
            {UserId = Guid.NewGuid().ToString(),
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "passw123@#",
                Role = "Customer",
                PhoneNumber = "1234567811",
                Address = "kolkata"
            };

            // Act
            var result = await _customerService.CreateCustomerAsync(newCustomer);

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("User Created Successfully", result.Message);
        }


        [Test]
        public async Task CreateCustomerAsync_ReturnsFailure_WhenCustomerExists()
        {
            // Arrange
            var existingCustomer = new Customer
            {
                UserId = Guid.NewGuid().ToString(), // Ensure UserId is set
                UserName = "existinguser",
                Email = "existinguser@example.com",
                Password = "password123",
                Role = "Customer",
                PhoneNumber = "1234567811",
                Address = "kolkata"
            };

            // Add an existing customer to the context
            _context.Customers.Add(existingCustomer);
            await _context.SaveChangesAsync();

            var newCustomer = new Customer
            {
                UserName = "existinguser", // Same UserName as the existing customer
                Email = "newuser@example.com",
                Password = "newpassword123",
                Role = "Customer",
                PhoneNumber = "1234567811",
                Address = "kolkata"
            };

            // Act
            var result = await _customerService.CreateCustomerAsync(newCustomer);

            // Assert
            Assert.AreEqual("Failure", result.Status);
            Assert.AreEqual("An Job Seeker with this username or email already exists.", result.Message);
        }


        [Test]
        public async Task DeleteCustomerAsync_ReturnsSuccess_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                UserId = "1234",
                UserName = "todeleteuser",
                Email = "todelete@example.com",
                Password = "password123",
                Role="Customer",
                PhoneNumber="1234567811",
                Address="kolkata"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.DeleteCustomerAsync("1234");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Deleted successfully.", result.Message);
            Assert.IsFalse(_context.Customers.Any(c => c.UserId == "1234"));
        }

        [Test]
        public async Task DeleteCustomerAsync_ReturnsFailure_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _customerService.DeleteCustomerAsync("nonexistentid");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Customer not found with the given ID.", result.Message);
        }

        [Test]
        public async Task GetCustomerByUserName_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                UserId = Guid.NewGuid().ToString(), // Ensure UserId is set
                UserName = "finduser",
                Email = "finduser@example.com",
                Password = "password123",
                Role = "Customer",
                PhoneNumber = "1234567811",
                Address = "kolkata"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomerByUserName("finduser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("finduser", result.UserName);
        }


        [Test]
        public async Task GetCustomerByUserName_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _customerService.GetCustomerByUserName("nonexistentuser");

            // Assert
            Assert.IsNull(result);
        }
    }
}
