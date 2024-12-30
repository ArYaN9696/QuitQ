using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface ICustomerService
    {
        public Task<Customer> GetCustomerByUserName(string userName);
        public Task<Response> CreateCustomerAsync(Customer customer);
        public Task<Response> DeleteCustomerAsync(string id);
    }
}
