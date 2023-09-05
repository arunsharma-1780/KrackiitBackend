using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateModuleValidator : AbstractValidator<Req.CreateModule>
    {
        public CreateModuleValidator()
        {
            RuleFor(prop => prop.ModuleType).IsInEnum().WithMessage("ModuleType must be [MockTest=1,Videos =2,Ebook =3,PreviousYearPaper =4,LiveClasses =5] is required!");
            RuleFor(prop => prop.ModuleName).NotNull().NotEmpty().WithMessage("ModuleName is required!");
        }
    }
    public class EditModuleValidator : AbstractValidator<Req.EditModule>
    {
        public EditModuleValidator()
        {
            RuleFor(prop => prop.Id).NotNull().NotEmpty().WithMessage("Id is required!");
            RuleFor(prop => prop.ModuleName).NotNull().NotEmpty().WithMessage("ModuleName is required!");
        }
    }
    public class GetModuleValidator : AbstractValidator<Req.GetModule>
    {
        public GetModuleValidator()
        {
            RuleFor(prop => prop.Id).NotNull().NotEmpty().WithMessage("Id is required!");
        }
    }
}
