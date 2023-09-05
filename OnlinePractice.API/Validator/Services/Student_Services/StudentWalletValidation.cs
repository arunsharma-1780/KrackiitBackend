using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class StudentWalletValidation : IStudentWalletValidation
    {
        public CreateStudentWalletValidator CreateStudentWalletValidator { get; set; } = new();

        public PurchaseFromWalletValidator PurchaseFromWalletValidator { get; set; } = new();
        public CheckoutValidator CheckoutValidator { get; set; } = new();
        public OderAmountValidator OderAmountValidator { get; set; } = new();
        public PaymentStatusValidator PaymentStatusValidator { get; set; } = new();
        public GetWalletHistoryValidator GetWalletHistoryValidator { get; set; } = new();

        public PaymentRequestValidator PaymentRequestValidator { get; set; } = new();

    }
}
