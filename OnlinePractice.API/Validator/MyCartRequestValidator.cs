using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class AddToCartValidator : AbstractValidator<Req.AddtoMyCart>
    {
        public AddToCartValidator()
        {
            RuleFor(prop => prop.ProductCategory).NotEmpty().NotNull().WithMessage("ProductCategory is required!");
            RuleFor(prop => prop.ProductId).NotEmpty().NotNull().WithMessage("ProductId is required!");
            RuleFor(prop => prop.Price).NotEmpty().NotNull().WithMessage("ProductCategory is required!").GreaterThan(0).WithMessage("Price must be greater than zero!");

        }
    }
    public class RemoveItemFromMyCartValidator : AbstractValidator<Req.RemoveItemFromMyCart>
    {
        public RemoveItemFromMyCartValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop.ProductId).NotEmpty().NotNull().WithMessage("ProductId is required!");
        }
    }
}
