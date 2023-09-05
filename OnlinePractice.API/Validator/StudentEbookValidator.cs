using FluentValidation;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class SubjectListByInstituteValidator : AbstractValidator<Req.EbooksSubjects>
    {
        public SubjectListByInstituteValidator()
        {
            
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.SubcourseId).NotEmpty().NotNull().WithMessage("SubcourseId is required!");
        }
    }

    public class EbookListValidator : AbstractValidator<Req.GetStudentEbook>
    {
        public EbookListValidator()
        {

            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.SubcourseId).NotEmpty().NotNull().WithMessage("SubcourseId is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
         //   RuleFor(prop => prop.TopicId).NotNull().WithMessage("TopicId is required!");
            RuleFor(prop => prop.LanguageFilter).IsInEnum().WithMessage("LanguageFilter Filter is must be [For All = 0,English = 1,Hindi = 2, Gujrati=3 ,Marathi=4 ]!");
            RuleFor(prop => prop.PriceWiseSort).IsInEnum().WithMessage("PriceFilter Filter is must be [For All = 0,HighToLow = 1,LowToHigh = 2]!");
            RuleFor(prop => prop.PriceFilter).IsInEnum().WithMessage("PricingFilter Filter is must be [For All = 0,Free = 1,Premium = 2]!");
            RuleFor(prop => prop.PageNumber).NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).NotEmpty().NotNull().WithMessage("PageSize is required!");
        }
    }
    public class GetEbookValidator : AbstractValidator<Req.GetEbookById>
    {
        public GetEbookValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("eBook-Id is required!");
        }
    }

}
