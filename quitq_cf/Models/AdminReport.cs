using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class AdminReport
    {
        public int ReportId { get; set; }
        public string? AdminId { get; set; }
        public string? ReportType { get; set; }
        public string? Content { get; set; }
        public DateTime? ReportDate { get; set; }

        public virtual Admin? Admin { get; set; }
    }
}
