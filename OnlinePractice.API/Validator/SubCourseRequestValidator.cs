using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateSubCourseValidator : AbstractValidator<Req.CreateSubCourse>
    {
        public CreateSubCourseValidator()
        {
            RuleFor(prop => prop.SubCourseName.Trim()).NotEmpty().WithMessage("SubCourseName must not be empty").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("SubCourse Name is invalid");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("SubCourse Id is required!");
        }
    }
    public class EditSubCourseValidator : AbstractValidator<Req.EditSubCourse>
    {
        public EditSubCourseValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubCourseId is required!!!");

            RuleFor(prop => prop).SetValidator(new CreateSubCourseValidator());
        }
    }

    public class EditMultipleSubCourseValidator : AbstractValidator<Req.EditMultipleSubCourse>
    {
        public EditMultipleSubCourseValidator()
        {
            RuleFor(prop => prop.CourseID).NotEmpty().NotNull().WithMessage("CourseID is required!!!");
            RuleFor(prop => prop.CourseName.Trim()).NotEmpty().WithMessage("CourseName must not be empty").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Course Name is invalid");
            When(prop => prop.SubCourses.Count> 0, () =>
            {
                RuleForEach(prop => prop.SubCourses).SetValidator(new SubCourseNameValidator());
            });

        }
    }


    public class SubCourseNameValidator : AbstractValidator<Req.SubCourses>
    {
        public SubCourseNameValidator()
        {
            RuleFor(prop => prop.SubCourseName.Trim()).NotEmpty().WithMessage("SubCourseName must not be empty").MaximumLength(150).WithMessage("Name must be not grater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+'*(){}/<>^$-]+\s?)*$").WithMessage("SubCourse Name is invalid"); ;
        }
    }

    public class GetSubCourseByIdValidator : AbstractValidator<Req.SubCourseById>
    {
        public GetSubCourseByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubCourseId is required!!!");
        }
    }

    public class DeleteSubCourseValidator : AbstractValidator<Req.SubCourseById>
    {
        public DeleteSubCourseValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("SubCourseId is required!!!");
        }
        
    }
    public class GetAllSubCourseByCourseIdValidator : AbstractValidator<Req.CourseById>
    {
        public GetAllSubCourseByCourseIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("CourseId is Required");
        }
    }

}
