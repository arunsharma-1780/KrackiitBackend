using FluentValidation;
using OnlinePractice.API.Models.Request;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{

    public class CreateSubjectValidator : AbstractValidator<Req.CreateSubject>
    {
        public CreateSubjectValidator()
        {
            RuleFor(prop => prop.SubjectName.Trim()).NotEmpty().NotNull().WithMessage("SubjectName must not be empty")
            .MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Subject Name is invalid");
        }
    }
    public class CreateSubjectCategoryValidator : AbstractValidator<Req.CreateSubjectCategory>
    {
        public CreateSubjectCategoryValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourse Id is required!");
            RuleForEach(prop => prop.SubjectIds).SetValidator(new SubjectByIdValidator());
        }
    }
    public class EditSubjectCategoryValidator : AbstractValidator<Req.EditSubjectCategory>
    {
        public EditSubjectCategoryValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourse Id is required!");
            RuleForEach(prop => prop.SubjectIds).SetValidator(new EditSubjectIdValidator());
        }
    }
    public class EditSubjectIdValidator : AbstractValidator<Req.EditSubjectId>
    {
        public EditSubjectIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Subject Id is required");
            RuleFor(prop => prop.Value).Must(x => x == false || x == true).WithMessage("Value is required!");
        }
    }
    public class SubjectByIdValidator : AbstractValidator<Req.SubjectById>
    {
        public SubjectByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Subject Id is required");
        }
    }
    public class EditSubjectValidator : AbstractValidator<Req.EditSubject>
    {
        public EditSubjectValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop=>prop).SetValidator(new CreateSubjectValidator());
        }
    }

    public class GetSubjectByIdValidator : AbstractValidator<Req.SubjectById>
    {
        public GetSubjectByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubjectId is required!!!");
        }
    }

    public class DeleteSubjectValidator : AbstractValidator<Req.SubjectById>
    {
        public DeleteSubjectValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubjectId is required!!!");
        }

    }
    public class GetAllSubjectBySubCourseIdValidator : AbstractValidator<Req.GetSubject>
    {
        public GetAllSubjectBySubCourseIdValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("CourseId is Required");
        }
    }

}
