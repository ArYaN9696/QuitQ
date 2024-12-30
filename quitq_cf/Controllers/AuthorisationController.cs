using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using quitq_cf.Repository;
using quitq_cf.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace quitq_cf.Controllers
{
    [EnableCors("AllowAny")]
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthorisationController : ControllerBase
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly ICustomerService _customerService;
        private readonly IAdminService _adminService;
        private readonly ISellerService _sellerService;
        private readonly IUserService _userService;
        public AuthorisationController(IAuthorisationService authorisationService, ICustomerService customerService, IAdminService adminService, ISellerService sellerService,IUserService userService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
            _customerService = customerService;
            _adminService = adminService;
            _sellerService = sellerService;
        }
        [Route("customer/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDTO registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "Passwords do not match!");
                return BadRequest(ModelState);
            }
            var createdUser = await _userService.RegisterCustomerAsync(registrationData);
            if (createdUser != null)
            {
                return Ok(new
                {
                    success = true,
                    message = "Customer registered successfully.",
                    data = createdUser
                });
            }
            else
            {
                ModelState.AddModelError("Error", "An error occuerd while creating the customer!");
                return StatusCode(500, new
                {
                    success = false,
                    ModelState
                });
            }
        }
        [Route("admin/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDTO registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "Passwords do not match!");
                return BadRequest(ModelState);
            }

            var createdAdmin = await _userService.RegisterAdminAsync(registrationData);
            if (createdAdmin != null)
            {
                return Ok(new
                {
                    success = true,
                    message = "Admin registered successfully.",
                    data = createdAdmin
                });
            }
            else
            {
                ModelState.AddModelError("Error", "An error occurred while creating the admin!");
                return StatusCode(500, new
                {
                    success = false,
                    ModelState
                });
            }
        }
        [Route("seller/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterSellerDTO registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (registrationData.Password != registrationData.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "Passwords do not match!");
                return BadRequest(ModelState);
            }

            var createdSeller = await _userService.RegisterSellerAsync(registrationData);
            if (createdSeller != null)
            {
                return Ok(new
                {
                    success = true,
                    message = "Seller registered successfully.",
                    data = createdSeller
                });
            }
            else
            {
                ModelState.AddModelError("Error", "An error occurred while creating the seller!");
                return StatusCode(500, new
                {
                    success = false,
                    ModelState
                });
            }
        }
        [Route("customer/login")]
        [HttpPost]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState);
            var user = await _customerService.GetCustomerByUserName(login.UserName);
            if (user == null) return BadRequest("Invalid UserName or Password!");
            var match = await _authorisationService.VerifyPasswordAsync(login.Password, user.Password);
            if (!match) return BadRequest("Invalid UserName or Password!");
            var token = await _userService.LoginAsync(user);
            return Ok(token);
        }
        [Route("admin/login")]
        [HttpPost]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState);
            var user = await _adminService.GetAdminByUserName(login.UserName);
            if (user == null) return BadRequest("Invalid UserName or Password!");
            var match = await _authorisationService.VerifyPasswordAsync(login.Password, user.Password);
            if (!match) return BadRequest("Invalid UserName or Password!");
            var token = await _userService.LoginAsync(user);
            return Ok(token);
        }
        [Route("seller/login")]
        [HttpPost]
        public async Task<IActionResult> SellerLogin([FromBody] LoginDTO login)
        {
            if (login == null) return BadRequest(ModelState);
            var user = await _sellerService.GetSellerByUserName(login.UserName);
            if (user == null) return BadRequest("Invalid UserName or Password!");
            var match = await _authorisationService.VerifyPasswordAsync(login.Password, user.Password);
            if (!match) return BadRequest("Invalid UserName or Password!");
            var token = await _userService.LoginAsync(user);
            return Ok(token);
        }
    }
}
