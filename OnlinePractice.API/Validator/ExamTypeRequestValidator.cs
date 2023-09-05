using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Validator
{
    public class CreateExamTypeValidator : AbstractValidator<Req.CreateExamType>
    {
        public CreateExamTypeValidator()
        {

            RuleFor(prop => prop.ExamName.Trim()).NotEmpty().WithMessage("ExamName is Required").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Exam Name is invalid");
;   
        }
    }
    public class EditExamTypeValidator : AbstractValidator<Req.EditExamType>
    {
        public EditExamTypeValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateExamTypeValidator());
        }
    }

    public class GetExamTypeByIdValidator : AbstractValidator<Req.ExamTypeById>
    {
        public GetExamTypeByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

    public class DeleteExamTypeValidator : AbstractValidator<Req.ExamTypeById>
    {
        public DeleteExamTypeValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }

    public class CreateExamFlowValidator : AbstractValidator<Req.CreateExamFlow>
    {
        public CreateExamFlowValidator()
        {
            RuleFor(prop => prop.ExamTypeName.Trim()).NotEmpty().NotNull().WithMessage("ExamTypeName is required!").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Exam Name is invalid");
            RuleFor(prop => prop.CourseName.Trim()).NotEmpty().NotNull().WithMessage("CourseName is required!").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Course Name is invalid");
            RuleFor(prop => prop.SubCourseName.Trim()).NotEmpty().NotNull().WithMessage("SubCourseName is required!").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("SubCourse Name is invalid");
            RuleForEach(prop => prop.SubjectIds).SetValidator(new SubjectCategoryValidator());
        }
    }

    public class SubjectCategoryValidator : AbstractValidator<Req.SubjectIds>
    {
        public SubjectCategoryValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
        }

    }

}

