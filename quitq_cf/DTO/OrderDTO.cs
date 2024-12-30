using quitq_cf.DTO;
namespace quitq_cf.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int StatusId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
        
    }
}
