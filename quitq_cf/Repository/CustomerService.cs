using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class CustomerService : ICustomerService
    {
        private readonly IAuthorisationService _authorisationServices;
        private readonly ApplicationDbContext _appDBContext;

        public CustomerService(IAuthorisationService authServices, ApplicationDbContext appDBContext)
        {
            _authorisationServices = authServices;
            _appDBContext = appDBContext;
        }

        public async Task<Response> CreateCustomerAsync(Customer customer)
        {
            try
            {
                var CustomerExists = await _appDBContext.Customers.FirstOrDefaultAsync(c => c.UserName == customer.UserName || c.Email == customer.Email);
                if (CustomerExists != null)
                {
                    return new Response { Status = "Failure", Message = "An Job Seeker with this username or email already exists." };
                }
                customer.Password = await _authorisationServices.HashPasswordAsync(customer.Password);
                customer.UserId = Guid.NewGuid().ToString();
                await _appDBContext.Customers.AddAsync(customer);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User Created Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error Try Again Later" };
            }
        }

        public async Task<Response> DeleteCustomerAsync(string id)
        {
            try
            {
                var customer = await _appDBContext.Customers.FirstOrDefaultAsync(customer => customer.UserId == id);
                if (customer == null)
                {
                    return new Response
                    {
                        Status = "Success",
                        Message = "Customer not found with the given ID."
                    };
                }
                _appDBContext.Customers.Remove(customer);
                await _appDBContext.SaveChangesAsync();
                return new Response
                {
                    Status = "Success",
                    Message = "Deleted successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the Customer: {ex.Message}"
                };
            }
        }

        public async Task<Customer> GetCustomerByUserName(string userName)
        {
            var existingCustomer = await _appDBContext.Customers.FirstOrDefaultAsync(c => c.UserName == userName);
            if (existingCustomer == null)
            {
                return null;
            }
            return existingCustomer;
        }
    }
}
