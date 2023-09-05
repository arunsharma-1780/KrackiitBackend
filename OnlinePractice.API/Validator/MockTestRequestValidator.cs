using FluentValidation;
using System.Data;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Validator
{
    #region  MockTestSetting Validations
    public class CreateMockTestSettingValidator : AbstractValidator<Req.CreateMockTestSetting>
    {
        public CreateMockTestSettingValidator()
        {

            RuleFor(prop => prop.MockTestName).NotEmpty().NotNull().WithMessage("MockTest Name is required!").Matches(@"^([a-zA-Z0-9_@.!%/|#&+,""';\?*=(){}/<>^$-]+\s?)*$").WithMessage("Invalid MockTest Name!");
            //RuleFor(prop => prop.TimeDurationHours).NotNull().WithMessage("Time Duration Hours is required").GreaterThanOrEqualTo(0).WithMessage("Time Duration Hours must be greater than or equal to zero");
            //RuleFor(prop => prop.TimeDurationMinutes).NotNull().WithMessage("Time Duration Minutes is required").GreaterThanOrEqualTo(0).WithMessage("Time Duration Minutes must be greater than or equal to zero");
            RuleFor(prop => prop.TimeDurationHours).GreaterThanOrEqualTo(0).WithMessage("Time Duration Hours must be greater than or equal to zero");
            When(prop => prop.TimeDurationHours.Equals(0), () =>
            {
                RuleFor(prop => prop.TimeDurationMinutes).GreaterThan(0).WithMessage("Time Duration hours or Time Duration minutes must not be 0 at the same time");
            });
            When(prop => prop.TimeDurationHours >= 0, () =>
            {
                RuleFor(prop => prop.TimeDurationMinutes).GreaterThanOrEqualTo(0).WithMessage("Time Duration Minutes must be greater than or equal to zero");
            });
            RuleFor(prop => prop.InstituteId).NotEmpty().WithMessage("InstituteId must no be Empty!").NotNull().WithMessage("InstituteId is required!");
            When(prop => prop.IsFree == false, () =>
            {
                RuleFor(prop => prop.Price).GreaterThan(0).WithMessage("Price must be greater than zero!").NotEmpty().NotNull().WithMessage("Price is required!");
            });
            RuleFor(prop => prop.TestAvailability).NotNull().IsInEnum().WithMessage("TestAvailability must be in [1,2]!");
            When(prop => prop.TestAvailability == Models.Enum.TestAvailability.Specific, () =>
            {
                RuleFor(prop => prop.TestAvailability).IsInEnum().WithMessage("TestAvailability must be in [1,2]!").NotEmpty().WithMessage("TestAvailability must not be empty").NotNull().WithMessage("TestAvailability must not be null");
                When(prop => prop.TestAvailability == Models.Enum.TestAvailability.Specific, () =>
                {

                    //RuleFor(prop => prop.TestSpecificFromDate).NotEmpty().WithMessage("Test Specific FromDate Required").GreaterThanOrEqualTo(prop => DateTime.UtcNow.Date).WithMessage("Test Specific FromDate must be greater or equal than CurrentDate ");
                    RuleFor(prop => prop.TestSpecificFromDate.GetValueOrDefault().ToString("MM/dd/yyyy")).NotEmpty().WithMessage("Test Start Time Required")
                    .GreaterThanOrEqualTo(prop => DateTime.UtcNow.ToString("MM/dd/yyyy")).WithMessage("TestSpecificFromDate Date must be greater than current date");
                    RuleFor(prop => prop.TestSpecificToDate).NotEmpty().WithMessage("Test Specific ToDate Required ")
                    .GreaterThanOrEqualTo(prop => prop.TestSpecificFromDate).WithMessage("Test Specific ToDate must be greater than TestSpecificFromDate");
                    RuleFor(prop => prop.TestStartTime).NotEmpty().WithMessage("Test Start Time Required").GreaterThan(prop => DateTime.UtcNow).WithMessage("Test Start time must be greater Current time ").
                    GreaterThanOrEqualTo(prop => prop.TestSpecificFromDate).WithMessage("Start time Date must be greater than TestSpecificFromDate");
                    RuleFor(prop => prop.TestStartTime).NotEmpty().WithMessage("Test Start Time Required").GreaterThan(prop => DateTime.Now).WithMessage("Test Start time must be greater Current time ");

                    RuleFor(prop => prop.TestStartTime.GetValueOrDefault().ToString("MM/dd/yyyy")).NotEmpty().WithMessage("Test Start Time Required")
                    .LessThanOrEqualTo(prop => prop.TestSpecificToDate.GetValueOrDefault().ToString("MM/dd/yyyy")).WithMessage("Start time Date must be less than TestSpecificToDate");
                    //RuleFor(x => x.TestSpecificToDate).GreaterThan(x => x.TestSpecificFromDate).WithMessage("To date must be greater than from date.");
                    //RuleFor(x => x).Must(x => (x.TestSpecificToDate - x.TestSpecificFromDate)).WithMessage("The date range cannot be more than 30 days.");
                });
                When(prop => prop.TestAvailability == Models.Enum.TestAvailability.Always, () =>
                {
                    //RuleFor(prop=>prop.TestStartTime).Must(x => x == string.Empty).WithMessage("TestSpecificFromDate must be null");
                    RuleFor(prop => prop.TestStartTime).Must(x => x == null).WithMessage("TestStartTime must be null");
                    When(prop => prop.TestSpecificFromDate != null, () =>
                    {
                        RuleFor(prop => prop.TestSpecificFromDate).Must(x => x == null).WithMessage("TestSpecificFromDate must be null");
                    });
                    When(prop => prop.TestSpecificToDate != null, () =>
                    {
                        RuleFor(prop => prop.TestSpecificToDate).Must(x => x == null).WithMessage("TestSpecificToDate must be null");
                    });
                });
                RuleFor(prop => prop.IsAllowReattempts).Must(x => x == false || x == true).NotNull().WithMessage("IsAllowReattempts must not be Empty");
            //    When(prop => prop.IsAllowReattempts == true, () =>
            //    {
            //        //RuleFor(prop => prop.IsUnlimitedAttempts).Must(x => x == false || x == true).NotNull().WithMessage("IsUnlimitedAttempts must not be Empty");

            //        //When(prop => prop.IsUnlimitedAttempts == false, () =>
            //        //{
            //        //    RuleFor(prop => prop.TotalAttempts).GreaterThan(0).WithMessage("When IsUnlimitedAttempts is false then total attempts must be greater than zero !!!");
            //        //});

            //        RuleFor(x => x).Must((model, dateRange, context) =>
            //{
            //    var range = (dateRange.TestSpecificToDate.Value.Day - dateRange.TestSpecificFromDate.Value.Day);
            //    return range == model.ReattemptsDays;
            //})
            //.WithMessage("Reattempt days range must be less than or equal to FromDate to ToDate");

            //        RuleFor(prop => prop.ReattemptsHours).GreaterThanOrEqualTo(0).WithMessage("Reattempts Hours must be greater than or equal to zero");
            //        RuleFor(prop => prop.ReattemptsMinutes).GreaterThanOrEqualTo(0).WithMessage("Reattempts Minutes must be greater than or equal to zero");
            //        //RuleFor(prop => prop.TestStartTime).LessThanOrEqualTo(x=>x.ReattemptsDays).WithMessage("Test Start Time must be less than reattempt days");
            //    });
                RuleFor(prop => prop.IsTestResume).Must(x => x == false || x == true).NotNull().WithMessage("IsTestResume must not be Empty");
                //When(prop => prop.IsTestResume == true, () =>
                //{
                //    RuleFor(prop => prop.IsUnlimitedResume).Must(x => x == false || x == true).NotNull().WithMessage("IsUnlimitedResume must not be Empty");
                //    When(prop => prop.IsUnlimitedResume == true, () =>
                //    {
                //        RuleFor(prop => prop.TotalResume).Equal(0);
                //    });
                //});
                //RuleFor(prop => prop.TestStartTime);
                //RuleFor(prop=>prop.TestStartTime).NotNull().IsInEnum().WithMessage(" BackButton must be in [1,2]!");
              //  RuleFor(prop => prop.BackButton).NotNull().IsInEnum().WithMessage(" BackButton must be in [1,2]!");
                RuleFor(prop => prop.IsMarksResultFormat).Must(x => x == false || x == true).NotNull().WithMessage("IsMarksResultFormat must not be Empty");
                RuleFor(prop => prop.IsPassFailResultFormat).Must(x => x == false || x == true).NotNull().WithMessage("IsPassFailResultFormat must not be Empty");
              //  RuleFor(prop => prop.IsRankResultFormat).Must(x => x == false || x == true).NotNull().WithMessage("IsRankResultFormat must not be Empty");
                RuleFor(prop => prop.ResultDeclaration).IsInEnum().WithMessage(" ResultDeclaration must be in [1,2]!");
                RuleFor(prop => prop.IsShowCorrectAnswer).Must(x => x == false || x == true).NotNull().WithMessage("IsShowCorrectAnswer must not be Empty");
                RuleFor(prop => prop.MockTestType).NotEmpty().IsInEnum().WithMessage("MockTestType must be in [1,2]!");
                RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("Language is required!");
            });
        }
    }

    public class EditMockTestSettingValidator : AbstractValidator<Req.EditMockTestSetting>
    {
        public EditMockTestSettingValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().WithMessage("Id must no be Empty!").NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateMockTestSettingValidator());
        }
    }
    public class GetMockTestSettingByIdValidator : AbstractValidator<Req.MocktestSettingById>
    {
        public GetMockTestSettingByIdValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().WithMessage("Id must no be Empty!").NotNull().WithMessage("Id is required!");
        }
    }
    public class DeleteMockTestSettingValidator : AbstractValidator<Req.MocktestSettingById>
    {
        public DeleteMockTestSettingValidator()
        {
            RuleFor(prop => prop).SetValidator(new GetMockTestSettingByIdValidator());
        }
    }
    public class MocktTestLogoValidator : AbstractValidator<Req.LogoImage>
    {
        public MocktTestLogoValidator()
        {
            RuleFor(prop => prop.Image.Length).NotNull().GreaterThanOrEqualTo(5000).WithMessage("Image should be greater than 5kb!");
            RuleFor(prop => prop.Image.Length).NotNull().LessThanOrEqualTo(16777216).WithMessage("Image should be less than 16mb!");

            RuleFor(prop => prop.Image).NotNull().Must(x => x.ContentType.Equals("image/jpeg") || x.ContentType.Equals("image/jpg") || x.ContentType.Equals("image/png") || x.ContentType.Equals("image/bmp") || x.ContentType.Equals("image/gif"))
                .WithMessage("Image allowed type are [jpeg, jpg, png, bmp, gif]");

        }
    }
    #endregion

    #region  MocktestQuestion Validations
    public class CreateMockTestQuestionsValidator : AbstractValidator<Req.CreateMockTestQuestionList>
    {
        public CreateMockTestQuestionsValidator()
        {
            RuleFor(prop => prop.MocktestSettingId).NotEmpty().NotNull().WithMessage("Mocktest Setting Id is required!");
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("Course Id is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");
            RuleFor(prop => prop.IsDraft).Must(x => x == false || x == true).WithMessage("IsDraft must be boolean").NotEmpty().NotNull().WithMessage("IsDraft is required!");
            RuleForEach(prop => prop.MockTestQuestions).SetValidator(new MockTestQuestionssValidator());
        }
    }
    public class MockTestQuestionssValidator : AbstractValidator<Req.MockTestQuestionss>
    {
        public MockTestQuestionssValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.SubjectName).NotEmpty().NotNull().WithMessage("SubjectName is required!");
            RuleFor(prop => prop.TotalQuestions).NotEmpty().WithMessage("TotalQuestions is Required").GreaterThan(0).WithMessage("TotalQuestions  must be greater than zero!");
            RuleFor(prop => prop.TotalAttempt).NotEmpty().WithMessage("TotalAttempt is Required").LessThanOrEqualTo(prop => prop.TotalQuestions).WithMessage("Total attempt cannot be greater than Total question !");
            RuleForEach(prop => prop.SectionDetails).SetValidator(new SectionDetailsValidator());
            RuleFor(prop => prop.NoOfQue).GreaterThanOrEqualTo(0).WithMessage("NoOfQue must be greater than or equal to zero").NotNull().WithMessage("NoOfQue is required");
        }
    }
    public class SectionDetailsValidator : AbstractValidator<Req.SectionDetails>
    {
        public SectionDetailsValidator()
        {
            RuleFor(prop => prop.SectionId).NotEmpty().NotNull().WithMessage("Section Id is required!");
            RuleFor(prop => prop.SectionName).NotEmpty().NotNull().WithMessage("Section Name is required!");
            RuleForEach(prop => prop.MockTestQuestions).SetValidator(new MockTestQuestionsValidator());
            RuleFor(prop => prop.TotalQuestions).NotEmpty().WithMessage("TotalQuestions is Required").GreaterThan(0).WithMessage("TotalQuestions  must be greater than zero!");
            RuleFor(prop => prop.TotalAttempt).NotEmpty().WithMessage("TotalAttempt is Required").LessThanOrEqualTo(prop => prop.TotalQuestions).WithMessage("Total attempt cannot be greater than Total question !");
        }
    }
    public class MockTestQuestionsValidator : AbstractValidator<Req.MockTestQuestions>
    {
        public MockTestQuestionsValidator()
        {
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.QuestionType).IsInEnum().WithMessage("QuestionType must be in [1,2,3,4,5] ").NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.QuestionLevel).NotEmpty().NotNull().WithMessage("QuestionLevel is required!");
            RuleFor(prop => prop.Mark).NotNull().GreaterThanOrEqualTo(0).WithMessage("Mark is required!");
            RuleFor(prop => prop.NegativeMark).NotNull().WithMessage("NegativeMark is required!");
        }
    }

    public class UpdateMockTestQuestionListValidator : AbstractValidator<Req.UpdateMockTestQuestionList>
    {
        public UpdateMockTestQuestionListValidator()
        {
            RuleFor(prop => prop.MocktestSettingId).NotEmpty().NotNull().WithMessage("Mocktest Setting Id is required!");
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("Course Id is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");
            RuleFor(prop => prop.IsDraft).Must(x => x == false || x == true).WithMessage("IsDraft must be boolean").NotEmpty().NotNull().WithMessage("IsDraft is required!");
            RuleForEach(prop => prop.MockTestQuestions).SetValidator(new MockTestQuestionssValidator());
        }
    }
    public class GetAllQuestionsValidator : AbstractValidator<Req.GetAllMockTestV1>
    {
        public GetAllQuestionsValidator()
        {
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("ExamTypeId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("CourseId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("ExamPatternId is required!");
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");
        }
    }
    public class GetAllQuestionsNewValidator : AbstractValidator<Req.GetAllQuestions>
    {
        public GetAllQuestionsNewValidator()
        {
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SubCourseId is required!");
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.TopicId).NotEmpty().NotNull().WithMessage("TopicId is required!");
            RuleFor(prop => prop.SubTopicId).NotEmpty().NotNull().WithMessage("SubTopicId is required!");
            RuleFor(prop => prop.QuestionType).NotEmpty().NotNull().WithMessage("QuestionType is required!");
            RuleFor(prop => prop.PageNumber).GreaterThanOrEqualTo(0).WithMessage("Page Number must be greater than or equal to zero").NotNull().WithMessage("Page Number is required!");
            RuleFor(prop => prop.PageSize).GreaterThanOrEqualTo(prop => prop.PageNumber).WithMessage("Page size must be greater than or equal to PageNumber").NotNull().WithMessage("Page Size is required!");
        }
    }
    public class MockTestByIdValidator : AbstractValidator<Req.MockTestById>
    {
        public MockTestByIdValidator()
        {
            RuleFor(prop => prop.MockTestSettingId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");

        }

    }
    public class QuestionListValidator : AbstractValidator<Req.QuestionList>
    {
        public QuestionListValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleFor(prop => prop.SectionId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.QuestionRefId).NotEmpty().NotNull().WithMessage("QuestionRefId is required!");
            RuleFor(prop => prop.Marks).NotNull().GreaterThanOrEqualTo(0).WithMessage("Marks is required!");
            RuleFor(prop => prop.NegativeMarks).NotNull().WithMessage("NegativeMarks is required!");
        }
    }
    public class AutomaticMockTestQuestionValidator : AbstractValidator<Req.AutomaticMockTestQuestion>
    {
        public AutomaticMockTestQuestionValidator()
        {
            RuleFor(prop => prop.MockTestSettingId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.QuestionLevel).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("MockTestSettingId is required!");
            RuleForEach(prop => prop.AutomaticMockTestQuestionsList).SetValidator(new AutomaticMockTestQuestionListValidator());
        }
    }
    public class AutomaticMockTestQuestionListValidator : AbstractValidator<Req.AutomaticMockTestQuestionsList>
    {
        public AutomaticMockTestQuestionListValidator()
        {
            RuleFor(prop => prop.SubjectId).NotEmpty().NotNull().WithMessage("SubjectId is required!");
            RuleForEach(prop => prop.SectionDetailss).SetValidator(new SectionDetailssValidator());

        }
    }
    public class SectionDetailssValidator : AbstractValidator<Req.SectionDetailss>
    {
        public SectionDetailssValidator()
        {
            RuleFor(prop => prop.SectionId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.SectionName).NotEmpty().NotNull().WithMessage("SectionName is required!");
            RuleFor(prop => prop.QuestionType).IsInEnum().WithMessage("QuestionType must be in [1,2,3,4,5] ").NotEmpty().NotNull().WithMessage("SubCourseId is required!");
        }
    }
    public class GetAutomaticMockTestQuestionValidator : AbstractValidator<Req.AutomaticMockTestQuestion>
    {
        public GetAutomaticMockTestQuestionValidator()
        {
            RuleFor(prop => prop).SetValidator(new AutomaticMockTestQuestionValidator());
        }
    }

    public class PublishAutoMaticMocktestQuestionValidator : AbstractValidator<Req.AutoMockTestQuestionList>
    {
        public PublishAutoMaticMocktestQuestionValidator()
        {
            RuleFor(prop => prop.MockTestSettingId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.ExamTypeId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.CourseId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.SubCourseId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.ExamPatternId).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.QuestionLevel).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleFor(prop => prop.Language).NotEmpty().NotNull().WithMessage("SectionId is required!");
            RuleForEach(prop => prop.MockTestQuestions).SetValidator(new MockTestQuestionssValidator());
        }
    }

    #endregion
}

