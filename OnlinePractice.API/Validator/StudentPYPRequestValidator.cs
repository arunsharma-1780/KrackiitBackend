using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class PYPListByInstituteValidator : AbstractValidator<Req.PYPInstitutes>
    {
        public PYPListByInstituteValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than zero").NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("Page size must be greater than  zero").NotEmpty().NotNull().WithMessage("PageSize is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.SubcourseId).NotEmpty().NotNull().WithMessage("SubcourseId is required!");
        }
    }
    public class PapersDataByFilterValidator : AbstractValidator<Req.StudentPreviousYearPaperFilter>
    {
        public PapersDataByFilterValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubcourseId is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.Year).NotNull().WithMessage("Year is not null!");
            RuleFor(prop => prop.LanguageFilter).IsInEnum().WithMessage("LanguageFilter Filter is must be [For All = 0,English = 1,Hindi = 2, Gujrati=3 ,Marathi=4 ]!");
            RuleFor(prop => prop.PricingFilter).IsInEnum().WithMessage("PricingFilter Filter is must be [For All = 0,Free = 1,Premium = 2]!");
            RuleFor(prop => prop.PriceWiseSort).IsInEnum().WithMessage("Price Wise Sort Filter is must be [For All = 0,HighToLow = 1,LowToHigh = 2]!");
            RuleFor(prop => prop.PageNumber).NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).NotEmpty().NotNull().WithMessage("PageSize is required!");





        }
    }


}
