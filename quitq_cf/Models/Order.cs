namespace quitq_cf.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int? StatusId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime? PaymentDate { get; set; } 

        public virtual OrderStatus? Status { get; set; }
        public virtual Customer? User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
