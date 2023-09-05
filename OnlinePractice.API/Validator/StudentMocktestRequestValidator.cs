using FluentValidation;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Request;
using Org.BouncyCastle.Ocsp;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class StudentGetMockTestsValidator : AbstractValidator<Req.StudentMockTest>
    {
        public StudentGetMockTestsValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than zero").NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("Page size must be greater than  zero").NotEmpty().NotNull().WithMessage("PageSize is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.LanguageFilter).IsInEnum().WithMessage("LanguageFilter is must be [For All = 0,English = 1,Hindi = 2,Gujarati = 3,Marathi = 4]!");
            RuleFor(prop => prop.StatusFilter).IsInEnum().WithMessage("Status Filter is must be [For All = 0,NotVisted = 1,InProgress = 2,Completed = 3,Expired = 4]!");
            RuleFor(prop => prop.PricingFilter).IsInEnum().WithMessage("PricingFilter Filter is must be [For All = 0,Free = 1,Premium = 2]!");
        }
    }

    public class CustomeStudentMockTestValidator : AbstractValidator<Req.CustomeStudentMockTest>
    {
        public CustomeStudentMockTestValidator()
        {
            RuleFor(prop => prop.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than zero").NotEmpty().NotNull().WithMessage("PageNumber is required!");
            RuleFor(prop => prop.PageSize).GreaterThan(0).WithMessage("Page size must be greater than  zero").NotEmpty().NotNull().WithMessage("PageSize is required!");
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.StatusFilter).IsInEnum().WithMessage("Status Filter is must be [For All = 0,NotVisted = 1,InProgress = 2,Completed = 3]!\"");
            RuleFor(prop => prop.LanguageFilter).IsInEnum().WithMessage("LanguageFilter is must be [For All = 0,English = 1,Hindi = 2,Gujarati = 3,Marathi = 4]!");

        }
    }

    public class StudentAutomaticMockTestQuestionValidator : AbstractValidator<Req.StudentAutomaticMockTestQuestion>
    {
        public StudentAutomaticMockTestQuestionValidator()
        {
            RuleFor(prop => prop.InstituteId).NotEmpty().NotNull().WithMessage("InstituteId is required!");
            RuleFor(prop => prop.MockTestName).NotEmpty().NotNull().WithMessage("MockTestName is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.QuestionLevel).IsInEnum().WithMessage("QuestionLevel is required[1 = Easy, 2 = Medium , 3 = Hard]!");
            RuleFor(prop => prop.Language).IsInEnum().WithMessage("Language is required! [1 = English, 2 = Hindi, 3 = Gujarati, 4 = Marathi ]");
        }
    }

    public class GetStudentQuestionPanelValidator : AbstractValidator<Req.GetStudentQuestionPanel>
    {
        public GetStudentQuestionPanelValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
        }
    }

    public class StudentQuestionResponseValidator : AbstractValidator<Req.StudentQuestionResponse>
    {
        public StudentQuestionResponseValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.QuestionType).IsInEnum().WithMessage("QuestionType is required!");
            RuleFor(prop => prop.IsCustome).Must(x => x == false || x == true).WithMessage("IsCustome is required!");
        }
    }

    public class MarkAsSeenValidator : AbstractValidator<Req.MarkAsSeen>
    {
        public MarkAsSeenValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.QuestionType).IsInEnum().WithMessage("QuestionType is required!");
            RuleFor(prop => prop.IsVisited).Must(x => x == false || x == true).WithMessage("IsVisited is required!");
            RuleFor(prop => prop.RemainingDuration).NotNull().WithMessage("RemainingDuration is required!");

        }
    }

    public class StudentAnwersPanelValidator : AbstractValidator<Req.StudentAnwersPanel>
    {
        public StudentAnwersPanelValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.IsCustome).Must(x => x == false || x == true).WithMessage("IsCustome is required!");
        }
    }

    public class ReviewAnswerValidator : AbstractValidator<Req.ReviewAnswer>
    {
        public ReviewAnswerValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.RemainingDuration).NotNull().WithMessage("RemainingDuration is required!");
        }
    }
    public class RemoveAnswerValidator : AbstractValidator<Req.RemoveAnswer>
    {
        public RemoveAnswerValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.RemainingDuration).NotNull().WithMessage("RemainingDuration is required!");
        }
    }
    public class StudentMockTestIdValidator : AbstractValidator<Req.StudentMockTestId>
    {
        public StudentMockTestIdValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");

        }
    }

    public class StudentMockTestStatusValidator : AbstractValidator<Req.StudentMockTestStatus>
    {
        public StudentMockTestStatusValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.IsStarted).Must(x => x == false || x == true).NotNull().WithMessage("IsStarted is required!");
            RuleFor(prop => prop.IsCompleted).Must(x => x == false || x == true).NotNull().WithMessage("IsCompleted is required!");
            RuleFor(prop => prop.IsCustome).Must(x => x == false || x == true).NotNull().WithMessage("IsCustome is required!");

        }
    }
    public class ResumeMockTestValidator : AbstractValidator<Req.ResumeMockTest>
    {
        public ResumeMockTestValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.IsPaused).Must(x => x == false || x == true).NotNull().WithMessage("IsStarted is required!");
            RuleFor(prop => prop.RemainingDuration).NotNull().WithMessage("RemainingDuration is required!");
        }
    }
    public class GetStudentResultValidator : AbstractValidator<Req.GetStudentResult>
    {
        public GetStudentResultValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.IsCustome).Must(x => x == false || x == true).NotNull().WithMessage("IsCustome is required!");


        }
    }
    public class GetResultValidator : AbstractValidator<Req.GetResult>
    {
        public GetResultValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
            RuleFor(prop => prop.UniqueMockTestId).NotEmpty().NotNull().WithMessage("UniqueMockTestId is required!");
            RuleFor(prop => prop.IsCustome).Must(x => x == false || x == true).NotNull().WithMessage("IsCustome is required!");
        }
    }

    public class GetStudentQuestionSolutionValidator : AbstractValidator<Req.GetStudentQuestionSolution>
    {
        public GetStudentQuestionSolutionValidator()
        {
            RuleFor(prop => prop.MockTestId).NotEmpty().NotNull().WithMessage("MockTestId is required!");
        }
    }
}