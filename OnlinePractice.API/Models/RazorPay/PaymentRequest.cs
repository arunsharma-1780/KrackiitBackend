using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.RazorPay
{
    public class PaymentRequest :CurrentUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public int Amount { get; set; }
    }
}
