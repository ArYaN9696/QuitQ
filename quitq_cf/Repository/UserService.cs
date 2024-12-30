using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Models;

namespace quitq_cf.Repository
{
    public class UserService : IUserService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly ICustomerService _customerService;
        private readonly IAdminService _adminService;
        private readonly ISellerService _sellerService;
        private readonly IMapper _mapper;

        public UserService(IAuthorisationService authorisationService, ICustomerService customerService, IMapper mapper, IAdminService adminService, ISellerService sellerService)
        {
            _authorisationService = authorisationService;
            _customerService = customerService;
            _mapper = mapper;
            _adminService = adminService;
            _sellerService = sellerService;
        }
        public async Task<TokenResponse?> LoginAsync<T>(T login)
        {
            var token = await _authorisationService.GenerateJWTTokenAsync(login);
            return token;
        }

        public async Task<Response> RegisterAdminAsync(RegisterAdminDTO regAdmin)
        {
            var adminToRegister = _mapper.Map<Admin>(regAdmin);
            var result = await _adminService.CreateAdminAsync(adminToRegister);
            return result;
        }

        public async Task<Response> RegisterCustomerAsync(RegisterCustomerDTO regCustomer)
        {
            var customerToRegister = _mapper.Map<Customer>(regCustomer);
            var result = await _customerService.CreateCustomerAsync(customerToRegister);
            return result;
            throw new NotImplementedException();
        }

        public async Task<Response> RegisterSellerAsync(RegisterSellerDTO regSeller)
        {
            var sellerToRegister = _mapper.Map<Seller>(regSeller);
            var result = await _sellerService.CreateSellerAsync(sellerToRegister);
            return result;
        }
    }
}
