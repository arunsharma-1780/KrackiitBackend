using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    #region Student Mocktests
    public class StudentMockTest :CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        public Guid InstituteId { get; set; }
        public LanguageFilter LanguageFilter { get; set; }
        public StatusFilter StatusFilter { get; set; }
        public PricingFilter PricingFilter { get; set; }

    }
    public class CustomeStudentMockTest : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        public Guid InstituteId { get; set; }
        public LanguageFilter LanguageFilter { get; set; }
        public CustomeStatusFilter StatusFilter { get; set; }
        //  public LanguageFilter LanguageFilter { get; set; }

    }
    #region Student AutomaticMockTest
    public class StudentAutomaticMockTestQuestion : CurrentUser
    {
        public string MockTestName { get; set; } = string.Empty;
        public Guid SubCourseId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid InstituteId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public QuestionLanguage Language { get; set; }

    }

    public class StudentAutomaticMockTestQuestionsList
    {
        public Guid SubjectId { get; set; }
        public List<SectionDetailss> SectionDetailss { get; set; } = new();
    }
    public class StudentSectionDetailss
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }

    }

    public class StudentMockTestById : CurrentUser
    {
        public Guid MockTestSettingId { get; set; }
    }

    public class StudentPublishMockTest
    {
        public Guid ExamPatternId { get; set; }
        public Guid MockTestSettingId { get; set; }
    }
    #endregion

    #endregion

    #region Student Question Panel
    public class GetStudentQuestionPanel : CurrentUser
    {
        public Guid MockTestId { get; set; }

        [DefaultValue(false)]
        public bool IsCustome { get; set; }
    }
    public class StudentQuestionResponse : CurrentUser
    {
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string StudentAnswer { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectD { get; set; }

        [DefaultValue(false)]
        public bool IsMarkReview { get; set; }

        [DefaultValue(false)]
        public bool IsCustome { get; set; }

        [DefaultValue(true)]
        public bool IsAnswered { get; set; }
        public TimeSpan RemainingDuration { get; set; }


    }
    public class MarkAsSeen : CurrentUser
    {
        public Guid MockTestId { get; set; }
        public QuestionType QuestionType { get; set; }
        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;

        [DefaultValue(false)]
        public bool IsVisited { get; set; }
        public TimeSpan RemainingDuration { get; set; }


    }
    public class StudentAnwersPanel : CurrentUser
    {
        public Guid MockTestId { get; set; }

        [DefaultValue(false)]
        public bool IsCustome { get; set; }
    }
    public class ReviewAnswer : CurrentUser
    {
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public TimeSpan RemainingDuration { get; set; }

    }
    public class RemoveAnswer : CurrentUser
    {
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public TimeSpan RemainingDuration { get; set; }

    }
    public class StudentMockTestId : CurrentUser
    {
        public Guid MockTestId { get; set; }
        public bool IsCustom { get; set; }

    }
    public class StudentMockTestStatus : CurrentUser
    {
        public Guid MockTestId { get; set; }

        [DefaultValue(false)]
        public bool IsCustome { get; set; }

        [DefaultValue(false)]
        public bool IsStarted { get; set; }
        [DefaultValue(false)]
        public bool IsCompleted { get; set; }       
    }
    public class ResumeMockTest: CurrentUser
    {
        public Guid MockTestId { get; set; }

        [DefaultValue(true)]
        public bool IsPaused { get; set; }
        public TimeSpan RemainingDuration { get; set; }

    }
    public class GetResult : CurrentUser
    {
        public Guid UniqueMockTestId { get; set; }
        public Guid MockTestId { get; set; }
        [DefaultValue(false)]
        public bool IsCustome { get; set; }
    }
    public class GetStudentResult : CurrentUser
    {
        public Guid MockTestId { get; set; }
        [DefaultValue(false)]
        public bool IsCustome { get; set; }
    }
    #endregion

    #region
    public class GetStudentQuestionSolution : CurrentUser
    {
        public Guid MockTestId { get; set; }

        [DefaultValue(false)]
        public bool IsCustome { get; set; }
    }
    #endregion
}
