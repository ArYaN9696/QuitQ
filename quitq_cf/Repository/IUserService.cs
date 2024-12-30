using quitq_cf.Data;
using quitq_cf.DTO;

namespace quitq_cf.Repository
{
    public interface IUserService
    {
        public Task<Response> RegisterCustomerAsync(RegisterCustomerDTO registerCustomer);
        public Task<Response> RegisterAdminAsync(RegisterAdminDTO regAdmin);
        public Task<Response> RegisterSellerAsync(RegisterSellerDTO regSeller);
        public Task<TokenResponse?> LoginAsync<T>(T login);
    }
}
