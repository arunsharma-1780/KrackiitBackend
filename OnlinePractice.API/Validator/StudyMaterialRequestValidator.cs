using FluentValidation;
using OnlinePractice.API.Models.Request;
using System.Text.RegularExpressions;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateStudyMaterialValidator : AbstractValidator<Req.CreateStudyMaterial>
    {
        public CreateStudyMaterialValidator()
        {
            RuleFor(prop => prop.Title).NotEmpty().NotNull().WithMessage("{PropertyName}  is  required!").Matches("^([a-zA-Z0-9]+\\s)*[a-zA-Z0-9]+$").WithMessage("Product Name is invalid! it will allow alphanumeric only!");
            RuleFor(prop => prop.Description).MaximumLength(500).WithMessage("Description must be less than 500 characters!");
            RuleFor(prop => prop.Price).NotEmpty().NotNull().WithMessage("  price is required!");
            RuleFor(prop => prop.ExamId).NotEmpty().NotNull().WithMessage("exam id required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("course id required!");
            RuleFor(prop => prop.FormatType).NotEmpty().NotNull().WithMessage("formatType is required!");
        }
    }
}
