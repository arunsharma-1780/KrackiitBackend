using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using Pay = OnlinePractice.API.Models.Payment.Request;
using PayRes = OnlinePractice.API.Models.Payment.Response;

namespace OnlinePractice.API.Validator
{
    public class CreateStudentWalletValidator : AbstractValidator<Req.CreateStudentWallet>
    {
        public CreateStudentWalletValidator()
        {
            RuleFor(prop => prop.OrderId).NotEmpty().NotNull().WithMessage("OrderId is required!");

        }
    }
    public class PurchaseFromWalletValidator : AbstractValidator<Req.PurchaseFromWallet>
    {
        public PurchaseFromWalletValidator()
        {
            RuleFor(prop => prop.Amount).NotNull().WithMessage("Amount is required!").GreaterThan(0).WithMessage("Amount must be greater than zero!");

        }
    }
    public class CheckoutValidator : AbstractValidator<Req.Checkout>
    {
        public CheckoutValidator()
        {
            RuleFor(prop => prop.TotalAmount).NotNull().WithMessage("TotalAmount is required!").GreaterThan(0).WithMessage("TotalAmount must be greater than zero!");
            RuleForEach(prop => prop.CheckOutItems).SetValidator(new CheckOutItemsValidator());

        }
    }
    public class CheckOutItemsValidator : AbstractValidator<Req.CheckOutItems>
    {
        public CheckOutItemsValidator()
        {
            RuleFor(prop => prop.ProductId).NotEmpty().NotNull().WithMessage("ProductId is required!");
            RuleFor(prop => prop.ProductCategory).IsInEnum().WithMessage("ProductCategory is  wrong!");
            RuleFor(prop => prop.Price).GreaterThan(0).WithMessage("Price must be greater than zero!");

        }
    }

    public class OderAmountValidator : AbstractValidator<Pay.OderAmount>
    {
        public OderAmountValidator()
        {
            RuleFor(prop => prop.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero!").NotNull().WithMessage("Amount is required!");

        }
    }
    public class PaymentStatusValidator : AbstractValidator<Pay.PaymentStatus>
    {
        public PaymentStatusValidator()
        {
            RuleFor(prop => prop.IsSuccess).Must(x => x == false || x == true).WithMessage("IsSuccess is required!");
            When(prop => prop.IsSuccess == true, () =>
            {
                RuleFor(prop => prop.OrderId).NotEmpty().NotNull().WithMessage("OrderId is required!");
            });
        }
    }
    public class GetWalletHistoryValidator : AbstractValidator<Req.GetWalletHistory>
    {
        public GetWalletHistoryValidator()
        {
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than zero!");
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than zero!");
        }
    }
    public class PaymentRequestValidator : AbstractValidator<Pay.PaymentRequest>
    {
       public PaymentRequestValidator()
        {
            RuleFor(prop => prop.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero!");
            RuleFor(prop => prop.IsMobile).Must(x=> x == true || x == false).WithMessage("IsMobile is required!");

        }
    }
}
