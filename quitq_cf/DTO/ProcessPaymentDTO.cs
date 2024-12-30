namespace quitq_cf.DTO
{
    public class ProcessPaymentDTO
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }

}
