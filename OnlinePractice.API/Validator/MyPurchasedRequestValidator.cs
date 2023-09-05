using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Validator
{

    public class CreateMyPurchasedValidator : AbstractValidator<Req.CreateMyPurchased>
    {
        public CreateMyPurchasedValidator()
        {
            RuleFor(prop => prop.ProductCategory).NotEmpty().NotNull().WithMessage("ProductCategory is required!");
            RuleFor(prop => prop.ProductId).NotEmpty().NotNull().WithMessage("ProductId is required!");
            RuleFor(prop => prop.Price).NotEmpty().NotNull().WithMessage("Price is required!");
            RuleFor(prop => prop.StudentId).NotEmpty().NotNull().WithMessage("StudentId is required!");
        }
    }



    public class GetMyPurchasedMocktestValidator : AbstractValidator<Req.MyPurchasedMocktest>
    {
        public GetMyPurchasedMocktestValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");           
        }
    }

    public class GetMyPurchasedEbooksValidator : AbstractValidator<Req.MyPurchasedEbook>
    {
        public GetMyPurchasedEbooksValidator()

        {
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");
        }
    }
    public class GetMyPurchasedVideosValidator : AbstractValidator<Req.MyPurchasedVideo>
    {
        public GetMyPurchasedVideosValidator()

        {
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");
        }
    }
    public class GetMyPurchasedPreviousYearPaperValidator : AbstractValidator<Req.MyPurchasedPreviousYearPAper>
    {
        public GetMyPurchasedPreviousYearPaperValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");
        }
    }


}
