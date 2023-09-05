using FluentValidation;
using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;
using Req = OnlinePractice.API.Models.Request;
using Org.BouncyCastle.Ocsp;

namespace OnlinePractice.API.Validator
{
    public class InstituteMockTestValidator : AbstractValidator<Req.MockTestInstitute>
    {
        public InstituteMockTestValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than zero").NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("Page size must be greater than  zero").NotEmpty().NotNull().WithMessage("PageSize is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
        }
    }

    
}
