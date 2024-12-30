using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class Payment
    {
        public string PaymentId { get; set; } = null!;
        public int? OrderId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }

        public virtual Order? Order { get; set; }
    }
}
