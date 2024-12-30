using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public interface ISellerService
    {
        public Task<Seller> GetSellerByUserName(string userName);
        public Task<Response> CreateSellerAsync(Seller seller);
        public Task<Response> DeleteSellerAsync(string id);
    }
}
