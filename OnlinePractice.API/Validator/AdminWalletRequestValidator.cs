using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
namespace OnlinePractice.API.Validator
{
    public class GetAdminWalletValidator : AbstractValidator<Req.GetAdminWallet>
    {
        public GetAdminWalletValidator()
        {
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than zero!");
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than zero!");
        }
    }
    public class GetTransactionDetailsValidator : AbstractValidator<Req.GetTransactionDetails>
    {
        public GetTransactionDetailsValidator()
        {
            RuleFor(prop => prop.OrderId).NotEmpty().NotNull().WithMessage("OrderId is required!");
        }
    }
}
