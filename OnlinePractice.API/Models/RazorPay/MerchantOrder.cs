namespace OnlinePractice.API.Models.RazorPay
{
    public class MerchantOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public string RazorpayKey { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string PaymentId { get; set; } = string.Empty;
    }
    
}
