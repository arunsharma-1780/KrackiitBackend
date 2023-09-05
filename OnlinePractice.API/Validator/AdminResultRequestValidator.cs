using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{

    public class GetMockTestListValidator : AbstractValidator<Req.GetMockTestList>
    {
        public GetMockTestListValidator()
        {
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
        }
    }
    public class GeAdminResultValidator : AbstractValidator<Req.GeAdminResult>
    {
        public GeAdminResultValidator()
        {
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("PageSize is must be greater than zero!");
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("PageNumber is must be greater than zero!");
        }
    }

    public class GeResultByMockTestIdValidator : AbstractValidator<Req.GeResultByMockTestId>
    {
        public GeResultByMockTestIdValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.StudentId).NotEmpty().NotNull().WithMessage("StudentId is required!");

        }
    }
    public class GeResultAnalysisDetailValidator : AbstractValidator<Req.GeResultAnalysisDetail>
    {
        public GeResultAnalysisDetailValidator()
        {
            RuleFor(prop => prop.StudentId).NotEmpty().NotNull().WithMessage("StudentId is required!");

        }
    }
}
