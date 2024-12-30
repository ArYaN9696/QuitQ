using System.ComponentModel.DataAnnotations;

namespace quitq_cf.DTO
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password should be longer than 8 characters")]
        public string Password { get; set; }
    }
}
