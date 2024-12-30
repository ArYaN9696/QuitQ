using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface IAdminService
    {
        public Task<Admin> GetAdminByUserName(string userName);
        public Task<Response> CreateAdminAsync(Admin admin);
        public Task<Response> DeleteAdminAsync(string id);
    }
}
