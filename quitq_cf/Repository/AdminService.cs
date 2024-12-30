using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class AdminService : IAdminService
    {
        private readonly IAuthorisationService _authorisationServices;
        private readonly ApplicationDbContext _appDBContext;

        public AdminService(IAuthorisationService authServices, ApplicationDbContext appDBContext)
        {
            _authorisationServices = authServices;
            _appDBContext = appDBContext;
        }

        public async Task<Response> CreateAdminAsync(Admin admin)
        {
            try
            {
                var adminExists = await _appDBContext.Admins.FirstOrDefaultAsync(a => a.UserName == admin.UserName || a.Email == admin.Email);
                if (adminExists != null)
                {
                    return new Response { Status = "Failure", Message = "An Admin with this username or email already exists." };
                }
                admin.Password = await _authorisationServices.HashPasswordAsync(admin.Password);
                admin.UserId = Guid.NewGuid().ToString();
                await _appDBContext.Admins.AddAsync(admin);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Admin Created Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error Try Again Later" };
            }
        }

        public async Task<Response> DeleteAdminAsync(string id)
        {
            try
            {
                var admin = await _appDBContext.Admins.FirstOrDefaultAsync(a => a.UserId == id);
                if (admin == null)
                {
                    return new Response
                    {
                        Status = "Failure",
                        Message = "Admin not found with the given ID."
                    };
                }
                _appDBContext.Admins.Remove(admin);
                await _appDBContext.SaveChangesAsync();
                return new Response
                {
                    Status = "Success",
                    Message = "Admin deleted successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the Admin: {ex.Message}"
                };
            }
        }

        public async Task<Admin> GetAdminByUserName(string userName)
        {
            var existingAdmin = await _appDBContext.Admins.FirstOrDefaultAsync(a => a.UserName == userName);
            return existingAdmin;
        }
    }
}
