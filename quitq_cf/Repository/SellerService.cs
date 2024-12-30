using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class SellerService : ISellerService
    {
        private readonly IAuthorisationService _authorisationServices;
        private readonly ApplicationDbContext _appDBContext;

        public SellerService(IAuthorisationService authServices, ApplicationDbContext appDBContext)
        {
            _authorisationServices = authServices;
            _appDBContext = appDBContext;
        }

        public async Task<Response> CreateSellerAsync(Seller seller)
        {
            try
            {
                var sellerExists = await _appDBContext.Sellers.FirstOrDefaultAsync(s => s.UserName == seller.UserName || s.Email == seller.Email);
                if (sellerExists != null)
                {
                    return new Response { Status = "Failure", Message = "A Seller with this username or email already exists." };
                }
                seller.Password = await _authorisationServices.HashPasswordAsync(seller.Password);
                seller.UserId = Guid.NewGuid().ToString();
                await _appDBContext.Sellers.AddAsync(seller);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Seller Created Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error Try Again Later" };
            }
        }

        public async Task<Response> DeleteSellerAsync(string id)
        {
            try
            {
                var seller = await _appDBContext.Sellers.FirstOrDefaultAsync(s => s.UserId == id);
                if (seller == null)
                {
                    return new Response
                    {
                        Status = "Failure",
                        Message = "Seller not found with the given ID."
                    };
                }
                _appDBContext.Sellers.Remove(seller);
                await _appDBContext.SaveChangesAsync();
                return new Response
                {
                    Status = "Success",
                    Message = "Seller deleted successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the Seller: {ex.Message}"
                };
            }
        }

        public async Task<Seller> GetSellerByUserName(string userName)
        {
            var existingSeller = await _appDBContext.Sellers.FirstOrDefaultAsync(s => s.UserName == userName);
            return existingSeller;
        }
    }
}
