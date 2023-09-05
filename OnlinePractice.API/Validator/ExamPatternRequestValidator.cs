using FluentValidation;
using OnlinePractice.API.Models.Request;
using Req = OnlinePractice.API.Models.Request;
using DM = OnlinePractice.API.Models.DBModel;

namespace OnlinePractice.API.Validator
{
    #region CreateExamPatternValidator
    public class CreateExamPatternValidator : AbstractValidator<Req.CreateExamPattern>
    {
        public CreateExamPatternValidator()
        {
            RuleFor(prop => prop.ExamPatternName.Trim()).NotEmpty().WithMessage("Exam Pattern Name is Required").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("ExamPattern Name is invalid");
            RuleForEach(x => x.Section).NotEmpty().WithMessage("ExamPatternSection is Required").NotNull().WithMessage("ExamPatternSection must not be null").SetValidator(new CreateExamPatternSectionValidator());
        }

    }
    public class CreateExamPatternSectionValidator : AbstractValidator<Req.Section>
    {
        public CreateExamPatternSectionValidator()
        {

            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleForEach(x => x.SubSection).NotEmpty().WithMessage("ExamPatternSubSection is Required").NotNull().WithMessage("ExamPatternSubSection must not be null").SetValidator(new CreateExamPatternSubSectionValidator());
        }

    }
    public class CreateExamPatternSubSectionValidator : AbstractValidator<Req.Subsection>
    {
        public CreateExamPatternSubSectionValidator()
        {
            RuleFor(prop => prop.TotalQuestions).NotEmpty().WithMessage("TotalQuestions is Required").GreaterThan(0).WithMessage("TotalQuestions  must be greater than zero!");
            RuleFor(prop => prop.TotalAttempt).NotEmpty().WithMessage("TotalAttempt is Required").LessThanOrEqualTo(prop => prop.TotalQuestions).WithMessage("Total attempt cannot be greater than Total question !");

        }
    }

    #endregion

    #region EditExampatternValidator
    public class EditExamPatternValidator : AbstractValidator<Req.EditExamPattern>
    {
        public EditExamPatternValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");
            RuleFor(prop => prop.ExamPatternName.Trim()).NotEmpty().WithMessage("ExamPatternName is Required").MaximumLength(150).WithMessage("Name must be not greater than 150.").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("ExamPattern Name is invalid");
            RuleForEach(prop => prop.Section).SetValidator(new EditSectionValidator());
        }
    }

    public class EditSectionValidator : AbstractValidator<Req.EditSection>
    {
        public EditSectionValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleForEach(prop => prop.SubSection).SetValidator(new EditExamPatternSection());
        }
    }

    public class EditExamPatternSection : AbstractValidator<Req.EditExamPatternSection>
    {
        public EditExamPatternSection()
        {
            RuleFor(prop => prop.TotalQuestions).NotEmpty().WithMessage("TotalQuestions is Required").GreaterThan(0).WithMessage("TotalQuestions  must be greater than zero!");
            RuleFor(prop => prop.TotalAttempt).NotEmpty().WithMessage("TotalAttempt is Required").LessThanOrEqualTo(prop => prop.TotalQuestions).WithMessage("Total attempt cannot be greater than Total question !");
        }
    }

    public class EditGeneralInstructionExamPatternValidator : AbstractValidator<Req.EditGeneralInstruction>
    {
        public EditGeneralInstructionExamPatternValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop.GeneralInstruction).NotEmpty().NotNull().WithMessage("GeneralInstruction is required!");
        }
    }
    #endregion

    #region GetExamPatternValidator
    public class GetByExamPatternIdValidator : AbstractValidator<Req.GetByExamPatternId>
    {
        public GetByExamPatternIdValidator()
        {

            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");

        }

    }
    public class GetSectionListValidator : AbstractValidator<Req.GetSectionList>
    {
        public GetSectionListValidator()
        {

            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");

        }

    }

    public class GetAllExamPatternValidator : AbstractValidator<Req.GetAllExamPattern>
    {
        public GetAllExamPatternValidator()
        {

            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("PageNumber must be greater and equal to zero").NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(0).WithMessage("PageSize must be greater and equal to zero").NotNull().WithMessage("PageSize is required!");

        }

    }

    #endregion

    #region DeleteExamPatternValidator
    public class DeleteExamPatternValidator : AbstractValidator<Req.GetExamPatternId>
    {
        public DeleteExamPatternValidator()
        {

            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");

        }
    }
    #endregion
}
