using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateCourseValidator : AbstractValidator<Req.CreateCourse>
    {
        public CreateCourseValidator()
        {
            RuleFor(prop=>prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseName.Trim()).NotEmpty().WithMessage("CourseName must not be empty").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Course Name is invalid");
        }
    }
    public class EditCourseValidator : AbstractValidator<Req.EditCourse>
    {
        public EditCourseValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateCourseValidator());
        }
    }

    public class GetCourseByIdValidator : AbstractValidator<Req.CourseById>
    {
        public GetCourseByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

    public class DeleteCourseValidator : AbstractValidator<Req.CourseById>
    {
        public DeleteCourseValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
    public class GetExamIdValidator : AbstractValidator<Req.GetExamId>
    {
        public GetExamIdValidator()
        {
            RuleFor(prop => prop.ExamId).NotEmpty().NotNull().WithMessage("ExamId is required!");
        }
    }
}
