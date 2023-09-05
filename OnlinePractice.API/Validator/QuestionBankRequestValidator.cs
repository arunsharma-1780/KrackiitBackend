using FluentValidation;
using Req = OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator

{

    #region CreateQuestionBankValidator
    public class CreateQuestionTableValidator : AbstractValidator<Req.QuestionTableData>
    {
        public CreateQuestionTableValidator()
        {
            RuleFor(prop => prop.QuestionRefId).NotNull().WithMessage("QuestionRefId must not be Empty");
            RuleFor(prop => prop.English).SetValidator(new CreateCommonQuestionValidator());
            RuleFor(prop => prop.Hindi).SetValidator(new CreateCommonQuestionValidator());
            RuleFor(prop => prop.Marathi).SetValidator(new CreateCommonQuestionValidator());
            RuleFor(prop => prop.Gujarati).SetValidator(new CreateCommonQuestionValidator());

        }
    }


    public class CreateCommonQuestionValidator : AbstractValidator<Req.QuestionCommon>
    {
        public CreateCommonQuestionValidator()
        {
            RuleFor(prop => prop.QuestionText).NotNull().WithMessage("QuestionText must not be Empty");
            RuleFor(prop => prop.OptionA).NotNull().WithMessage("OptionA must not be Empty");
            RuleFor(prop => prop.IsCorrectA).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectA must not be Empty");
            RuleFor(prop => prop.OptionB).NotNull().WithMessage("OptionB must not be Empty");
            RuleFor(prop => prop.IsCorrectB).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectB must not be Empty");
            RuleFor(prop => prop.OptionC).NotNull().WithMessage("OptionC must not be Empty");
            RuleFor(prop => prop.IsCorrectC).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectC must not be Empty");
            RuleFor(prop => prop.OptionD).NotNull().WithMessage("OptionD must not be Empty");
            RuleFor(prop => prop.IsCorrectD).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectD must not be Empty");
            RuleFor(prop => prop.Explanation).NotNull().WithMessage("Explanation must not be Empty");
        }
    }

    public class CreateQuestionBankValidator : AbstractValidator<Req.CreateQuestionBank>
    {
        public CreateQuestionBankValidator()
        {

            RuleFor(prop => prop.TopicId).NotNull().WithMessage("TopicId is required!");
            RuleFor(prop => prop.SubTopicId).NotNull().WithMessage("SubTopicId is required!");
            RuleFor(prop => prop.QuestionType).NotNull().IsInEnum().WithMessage("QuestionType must be in [1,2,3,4,5]!");
            When(prop => prop.QuestionType == Models.Enum.QuestionType.SingleChoice, () =>
            {
                RuleFor(prop => prop.IsPartiallyCorrect).Must(x => x == false).WithMessage("IsPartiallyCorrect must be false");
                RuleFor(prop => prop.PartialThreeCorrectMark).Equal(0).WithMessage("PartialThreeCorrectMark must be zero");
                RuleFor(prop => prop.PartialTwoCorrectMark).Equal(0).WithMessage("PartialTwoCorrectMark must be zero");
                RuleFor(prop => prop.PartialOneCorrectMark).Equal(0).WithMessage("PartialOneCorrectMark must be zero");
            });

            When(prop => prop.QuestionType == Models.Enum.QuestionType.MCQ, () =>
            {
                RuleFor(prop => prop.IsPartiallyCorrect).NotNull().WithMessage("IsPartially !");
                When(prop => prop.IsPartiallyCorrect == true, () =>
                {
                    RuleFor(prop => prop.PartialThreeCorrectMark).NotNull().WithMessage("PartialThreeCorrectMark is  required!").GreaterThan(0).WithMessage("PartialThreeCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialThreeCorrectMark must be less than marks given");
                    RuleFor(prop => prop.PartialTwoCorrectMark).NotNull().WithMessage("PartialTwoCorrectMark is  required!").GreaterThan(0).WithMessage("PartialTwoCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialTwoCorrectMark must be less than marks given");
                    RuleFor(prop => prop.PartialOneCorrectMark).NotNull().WithMessage("PartialOneCorrectMark is  required!").GreaterThan(0).WithMessage("PartialOneCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialOneCorrectMark must be less than marks given");
                }
                );
                RuleFor(prop => prop.IsPartiallyCorrect).NotNull().WithMessage("IsPartially !");
                When(prop => prop.IsPartiallyCorrect == false, () =>
                {
                    RuleFor(prop => prop.PartialThreeCorrectMark).Equal(0).WithMessage("PartialThreeCorrectMark Equals to zero");
                    RuleFor(prop => prop.PartialTwoCorrectMark).Equal(0).WithMessage("PartialTwoCorrectMark Equals to zero");
                    RuleFor(prop => prop.PartialOneCorrectMark).Equal(0).WithMessage("PartialOneCorrectMark Equals to zero");
                }
                );

            });
            RuleFor(prop => prop.QuestionLevel).NotNull().IsInEnum().WithMessage("QuestionLevel must be in [1,2,3]!");
            RuleFor(prop => prop.QuestionTableData).SetValidator(new CreateQuestionTableValidator());
            RuleFor(prop => prop.Mark).NotNull().WithMessage("Total mark is required!").GreaterThan(0).WithMessage("Total marks must be greater than 0!");
            RuleFor(prop => prop.NegativeMark).NotNull().WithMessage("NegativeMark is required!");
            RuleFor(prop => prop.IsPartiallyCorrect).Must(x => x == false || x == true).NotNull().WithMessage("IsPartiallyCorrect must not be Empty");
            RuleFor(prop => prop.PartialThreeCorrectMark).NotNull().WithMessage("PartialThreeCorrectMark is required!");
            RuleFor(prop => prop.PartialTwoCorrectMark).NotNull().WithMessage("PartialTwoCorrectMark is required!");
            RuleFor(prop => prop.PartialOneCorrectMark).NotNull().WithMessage("PartialOneCorrectMark is required!");

        }
    }
    #endregion

    #region EditQuestionBankValidator
    public class EditQuestionBankValidator : AbstractValidator<Req.EditQuestionBank>
    {
        public EditQuestionBankValidator()
        {
            RuleFor(prop => prop.TopicId).NotNull().WithMessage("TopicId is required!");
            RuleFor(prop => prop.SubTopicId).NotNull().WithMessage("SubTopicId is required!");
            RuleFor(prop => prop.QuestionType).NotNull().IsInEnum().WithMessage("QuestionType must be in [1,2,3,4,5]!");
            RuleFor(prop => prop.QuestionLevel).NotNull().IsInEnum().WithMessage("QuestionLevel must be in [1,2,3]!");
            RuleFor(prop => prop.QuestionTableData).SetValidator(new EditQuestionTableData());
            RuleFor(prop => prop.Mark).NotNull().WithMessage("Total marks is required!").GreaterThan(0).WithMessage("Total marks must be greater than 0!");
            RuleFor(prop => prop.NegativeMark).NotNull().WithMessage("NegativeMark is required!");
            RuleFor(prop => prop.IsPartiallyCorrect).Must(x => x == false || x == true).NotNull().WithMessage("IsPartiallyCorrect must not be Empty");
            RuleFor(prop => prop.PartialThreeCorrectMark).NotNull().WithMessage("PartialThreeCorrectMark is required!");
            RuleFor(prop => prop.PartialTwoCorrectMark).NotNull().WithMessage("PartialTwoCorrectMark is required!");
            RuleFor(prop => prop.PartialOneCorrectMark).NotNull().WithMessage("PartialOneCorrectMark is required!");
            When(prop => prop.QuestionType == Models.Enum.QuestionType.SingleChoice, () =>
            {
                RuleFor(prop => prop.IsPartiallyCorrect).Must(x => x == false).WithMessage("IsPartiallyCorrect must be false");
                RuleFor(prop => prop.PartialThreeCorrectMark).Equal(0).WithMessage("PartialThreeCorrectMark must be zero");
                RuleFor(prop => prop.PartialTwoCorrectMark).Equal(0).WithMessage("PartialTwoCorrectMark must be zero");
                RuleFor(prop => prop.PartialOneCorrectMark).Equal(0).WithMessage("PartialOneCorrectMark must be zero");
            });

            When(prop => prop.QuestionType == Models.Enum.QuestionType.MCQ, () =>
            {
                RuleFor(prop => prop.IsPartiallyCorrect).NotNull().WithMessage("IsPartially !");
                When(prop => prop.IsPartiallyCorrect == true, () =>
                {
                    RuleFor(prop => prop.PartialThreeCorrectMark).NotNull().WithMessage("PartialThreeCorrectMark is  required!").GreaterThan(0).WithMessage("PartialThreeCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialThreeCorrectMark must be less than marks given");
                    RuleFor(prop => prop.PartialTwoCorrectMark).NotNull().WithMessage("PartialTwoCorrectMark is  required!").GreaterThan(0).WithMessage("PartialTwoCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialTwoCorrectMark must be less than marks given");
                    RuleFor(prop => prop.PartialOneCorrectMark).NotNull().WithMessage("PartialOneCorrectMark is  required!").GreaterThan(0).WithMessage("PartialOneCorrectMark must be greater than zero").LessThan(prop => prop.Mark).WithMessage("PartialOneCorrectMark must be less than marks given");
                }
                );
                RuleFor(prop => prop.IsPartiallyCorrect).NotNull().WithMessage("IsPartially !");
                When(prop => prop.IsPartiallyCorrect == false, () =>
                {
                    RuleFor(prop => prop.PartialThreeCorrectMark).Equal(0).WithMessage("PartialThreeCorrectMark Equals to zero");
                    RuleFor(prop => prop.PartialTwoCorrectMark).Equal(0).WithMessage("PartialTwoCorrectMark Equals to zero");
                    RuleFor(prop => prop.PartialOneCorrectMark).Equal(0).WithMessage("PartialOneCorrectMark Equals to zero");
                }
                );

            });
            //When(x => x.IsPartiallyCorrect == true, () =>
            //{
            //    RuleFor(prop => prop.PartialThreeCorrectMark).GreaterThan(0).WithMessage("PartialThreeCorrectMark must be greater than zero");
            //    RuleFor(prop => prop.PartialTwoCorrectMark).GreaterThan(0).WithMessage("PartialTwoCorrectMark must be greater than zero");
            //    RuleFor(prop => prop.PartialOneCorrectMark).GreaterThan(0).WithMessage("PartialOneCorrectMark must be greater than zero");
            //});
        }

    }

    public class EditQuestionTableData : AbstractValidator<Req.EditQuestionTableData>
    {
        public EditQuestionTableData()
        {
            RuleFor(prop => prop.QuestionRefId).NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.English).SetValidator(new EnglishEditCommonQuestionValidator());
            RuleFor(prop => prop.Hindi).SetValidator(new HindiEditCommonQuestionValidator());
            RuleFor(prop => prop.Marathi).SetValidator(new MarathiEditCommonQuestionValidator());
            RuleFor(prop => prop.Gujarati).SetValidator(new GujaratiEditCommonQuestionValidator());
        }

    }
    public class EnglishEditCommonQuestionValidator : AbstractValidator<Req.EnglishEdit>
    {
        public EnglishEditCommonQuestionValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id is Required");
            RuleFor(prop => prop.QuestionText).NotNull().WithMessage("QuestionText must not be Empty");
            RuleFor(prop => prop.OptionA).NotNull().WithMessage("OptionA must not be Empty");
            RuleFor(prop => prop.IsCorrectA).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectA must not be Empty");
            RuleFor(prop => prop.OptionB).NotNull().WithMessage("OptionB must not be Empty");
            RuleFor(prop => prop.IsCorrectB).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectB must not be Empty");
            RuleFor(prop => prop.OptionC).NotNull().WithMessage("OptionC must not be Empty");
            RuleFor(prop => prop.IsCorrectC).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectC must not be Empty");
            RuleFor(prop => prop.OptionD).NotNull().WithMessage("OptionD must not be Empty");
            RuleFor(prop => prop.IsCorrectD).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectD must not be Empty");
            RuleFor(prop => prop.Explanation).NotNull().WithMessage("Explanation must not be Empty");
        }
    }
    public class HindiEditCommonQuestionValidator : AbstractValidator<Req.HindiEdit>
    {
        public HindiEditCommonQuestionValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id is Required");
            RuleFor(prop => prop.QuestionText).NotNull().WithMessage("QuestionText must not be Empty");
            RuleFor(prop => prop.OptionA).NotNull().WithMessage("OptionA must not be Empty");
            RuleFor(prop => prop.IsCorrectA).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectA must not be Empty");
            RuleFor(prop => prop.OptionB).NotNull().WithMessage("OptionB must not be Empty");
            RuleFor(prop => prop.IsCorrectB).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectB must not be Empty");
            RuleFor(prop => prop.OptionC).NotNull().WithMessage("OptionC must not be Empty");
            RuleFor(prop => prop.IsCorrectC).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectC must not be Empty");
            RuleFor(prop => prop.OptionD).NotNull().WithMessage("OptionD must not be Empty");
            RuleFor(prop => prop.IsCorrectD).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectD must not be Empty");
            RuleFor(prop => prop.Explanation).NotNull().WithMessage("Explanation must not be Empty");
        }
    }
    public class GujaratiEditCommonQuestionValidator : AbstractValidator<Req.GujaratiEdit>
    {
        public GujaratiEditCommonQuestionValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id is Required");
            RuleFor(prop => prop.QuestionText).NotNull().WithMessage("QuestionText must not be Empty");
            RuleFor(prop => prop.OptionA).NotNull().WithMessage("OptionA must not be Empty");
            RuleFor(prop => prop.IsCorrectA).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectA must not be Empty");
            RuleFor(prop => prop.OptionB).NotNull().WithMessage("OptionB must not be Empty");
            RuleFor(prop => prop.IsCorrectB).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectB must not be Empty");
            RuleFor(prop => prop.OptionC).NotNull().WithMessage("OptionC must not be Empty");
            RuleFor(prop => prop.IsCorrectC).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectC must not be Empty");
            RuleFor(prop => prop.OptionD).NotNull().WithMessage("OptionD must not be Empty");
            RuleFor(prop => prop.IsCorrectD).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectD must not be Empty");
            RuleFor(prop => prop.Explanation).NotNull().WithMessage("Explanation must not be Empty");
        }
    }
    public class MarathiEditCommonQuestionValidator : AbstractValidator<Req.MarathiEdit>
    {
        public MarathiEditCommonQuestionValidator()
        {
            RuleFor(prop => prop.Id).NotNull().WithMessage("Id is Required");
            RuleFor(prop => prop.QuestionText).NotNull().WithMessage("QuestionText must not be Empty");
            RuleFor(prop => prop.OptionA).NotNull().WithMessage("OptionA must not be Empty");
            RuleFor(prop => prop.IsCorrectA).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectA must not be Empty");
            RuleFor(prop => prop.OptionB).NotNull().WithMessage("OptionB must not be Empty");
            RuleFor(prop => prop.IsCorrectB).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectB must not be Empty");
            RuleFor(prop => prop.OptionC).NotNull().WithMessage("OptionC must not be Empty");
            RuleFor(prop => prop.IsCorrectC).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectC must not be Empty");
            RuleFor(prop => prop.OptionD).NotNull().WithMessage("OptionD must not be Empty");
            RuleFor(prop => prop.IsCorrectD).Must(x => x == false || x == true).NotNull().WithMessage("IsCorrectD must not be Empty");
            RuleFor(prop => prop.Explanation).NotNull().WithMessage("Explanation must not be Empty");
        }
    }

    #endregion

    #region GetQuestionBankValidator
    public class GetByReferenceIdQuestionBankValidator : AbstractValidator<Req.GetQuestionBank>
    {
        public GetByReferenceIdQuestionBankValidator()
        {
            RuleFor(prop => prop.QuestionRefId).NotNull().WithMessage("QuestionRefId is required!");
        }
    }

    public class GetAllQuestionBankValidator : AbstractValidator<Req.GetAllQuestion>
    {
        public GetAllQuestionBankValidator()
        {
            //RuleFor(prop => prop.SubjectCategoryId).NotNull().WithMessage("SubjectCategoryId is required!");
            //RuleFor(prop => prop.TopicId).NotNull().WithMessage("TopicId is required!");
            //RuleFor(prop => prop.SubTopicId).NotNull().WithMessage("SubTopicId is required!");
            //RuleFor(prop => prop.QuestionType).NotNull().IsInEnum().WithMessage("QuestionType must be in [1,2,3,4,5]!");
            //RuleFor(prop => prop.CreatorUserId).NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.PageNumber).NotNull().WithMessage("PageNumber is required!").GreaterThanOrEqualTo(0).WithMessage("PageNumber must be greater than zero");
            RuleFor(prop => prop.PageSize).NotNull().WithMessage("PageSize is required!").GreaterThanOrEqualTo(0).WithMessage("PageSize must be greater than zero");
        }
    }

    #endregion

    #region DeleteQuestionBankValidator
    public class DeleteQuestionBankValidator : AbstractValidator<Req.QuestionBankRefId>
    {
        public DeleteQuestionBankValidator()
        {
            RuleFor(prop => prop.QuestionRefId).NotNull().WithMessage("QuestionRefId is required!");
        }
    }
    #endregion



}
