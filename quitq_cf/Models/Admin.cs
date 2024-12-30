using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class Admin
    {
        public Admin()
        {
            AdminReports = new HashSet<AdminReport>();
        }

        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Role { get; set; } = null!;

        public virtual ICollection<AdminReport> AdminReports { get; set; }
    }
}
