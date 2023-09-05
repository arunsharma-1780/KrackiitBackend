namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IStudentWalletValidation
    {
        public CreateStudentWalletValidator CreateStudentWalletValidator { get; set; }
        public PurchaseFromWalletValidator PurchaseFromWalletValidator { get; set; }
        public CheckoutValidator CheckoutValidator { get; set; }
        public OderAmountValidator OderAmountValidator { get; set; }
        public PaymentStatusValidator PaymentStatusValidator { get; set; }
        public GetWalletHistoryValidator GetWalletHistoryValidator { get; set; }
        public PaymentRequestValidator PaymentRequestValidator { get; set; }
    }
}
