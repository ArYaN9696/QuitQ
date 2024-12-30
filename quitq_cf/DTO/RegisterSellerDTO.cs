using System.ComponentModel.DataAnnotations;

namespace quitq_cf.DTO
{
    public class RegisterSellerDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username must be longer than 3 characters")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
