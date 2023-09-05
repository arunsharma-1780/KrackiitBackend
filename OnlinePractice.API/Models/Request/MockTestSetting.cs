using EllipticCurve;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Response;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    #region MockTestSettings
    public class CreateMockTestSetting : CurrentUser
    {

        #region BasicDetails
        public string MockTestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        #endregion

        #region Pricing
        public bool IsFree { get; set; }
        public double Price { get; set; }
        #endregion

        #region TimeSettings
        public int TimeDurationHours { get; set; }
        public int TimeDurationMinutes { get; set; }
        public TestAvailability TestAvailability { get; set; }//Specific = 1,Always = 2
        public DateTime? TestStartTime { get; set; }
        public DateTime? TestSpecificToDate { get; set; }
        public DateTime? TestSpecificFromDate { get; set; }
        #endregion

        #region AttemptSettings
        public bool IsAllowReattempts { get; set; }
      //  public bool IsUnlimitedAttempts { get; set; }
        public int TotalAttempts { get; set; }
        //public int ReattemptsDays { get; set; }
        //public int ReattemptsHours { get; set; }
        //public int ReattemptsMinutes { get; set; }
        #endregion

        #region ResumeSettings
        public bool IsTestResume { get; set; }
        //public bool IsUnlimitedResume { get; set; }
        //public int TotalResume { get; set; }
        #endregion

        #region Appearance
     //   public BackButton BackButton { get; set; } // 1:Allowed ,2:NotAllowed
        #endregion

        #region ResultDeclarationOption
        public bool IsMarksResultFormat { get; set; }
        public bool IsPassFailResultFormat { get; set; }
     //   public bool IsRankResultFormat { get; set; }

        public ResultDeclaration ResultDeclaration { get; set; }
        public bool IsShowCorrectAnswer { get; set; }
        #endregion

        public bool IsDraft { get; set; }
        #region Select MockTestType
        public MockTestType MockTestType { get; set; }
        #endregion

        #region Select Language
        public string Language { get; set; } = string.Empty;
        public string SecondaryLanguage { get; set; } = string.Empty;
        #endregion
    }

    public class Languages
    {
        public string Language { get; set; } = string.Empty;
    }

    public class CheckInstitute
    {
        public Guid InstituteId { get; set; }
    }
    public class CheckMockTestById
    {
        public Guid MockTestSettingId { get; set; }
    }

    public class EditMockTestSetting : CreateMockTestSetting
    {
        public Guid Id { get; set; }
    }
    public class MocktestSettingById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetAllMockTest : CurrentUser
    {

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }

    }
    public class GetAllMockTestV1 : CurrentUser
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
        public Guid ExamPatternId { get; set; }
        public string Language { get; set; } = string.Empty;

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }

    }
    public class GetAllQuestions
    {
        public Guid SubCourseId { get; set; }

        public Guid SubjectId { get; set; }

        public Guid TopicId { get; set; }

        public Guid SubTopicId { get; set; }

        public QuestionType QuestionType { get; set; }
        public string Language { get; set; } = string.Empty;

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class MockTestNameCheck
    {
        public string MockTestName { get; set; } = string.Empty;
    }
    public class CustomeMockTestNameCheck :CurrentUser
    {
        public string MockTestName { get; set; } = string.Empty;
    }
    public class EditMockTestNameCheck
    {
        public Guid Id { get; set; }
        public string MockTestName { get; set; } = string.Empty;
    }
    #endregion

    #region MockTestQuestion
    public class CreateMockTestQuestions : CurrentUser
    {
        public Guid MocktestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public List<QuestionList> QuestionList { get; set; } = new();
    }
    public class QuestionList
    {
        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public int Marks { get; set; }
        public int NegativeMarks { get; set; }
    }

    public class CreateMockTestQuestionList : CurrentUser
    {
        public Guid MocktestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public bool IsDraft { get; set; }
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();

    }
    public class UpdateMockTestQuestionList : CurrentUser
    {
        public Guid MocktestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public Guid SectionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsDraft { get; set; }
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();

    }
    public class MockTestQuestionss
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
        public List<SectionDetails> SectionDetails { get; set; } = new();
        public int NoOfQue { get; set; }
    }
    public class SectionDetails
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public List<MockTestQuestions> MockTestQuestions { get; set; } = new();
        public int TotalAttempt { get; set; }
        public int TotalQuestions { get; set; }

    }
    public class MockTestQuestions
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public string QuestionLevel { get; set; } = string.Empty;//Easy=1,Medium=2,Hard=3
        public int Mark { get; set; }
        public int NegativeMark { get; set; }

    }
    #endregion

    #region AutomaticMockTest
    public class AutomaticMockTestQuestion
    {
        public Guid MockTestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<AutomaticMockTestQuestionsList> AutomaticMockTestQuestionsList { get; set; } = new();
    }

    public class AutomaticMockTestQuestionsList
    {
        public Guid SubjectId { get; set; }
        public List<SectionDetailss> SectionDetailss { get; set; } = new();
    }
    public class SectionDetailss
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }

    }

    public class MockTestById : CurrentUser
    {
        public Guid MockTestSettingId { get; set; }
    }

    public class PublishMockTest
    {
        public Guid ExamPatternId { get; set; }
        public Guid MockTestSettingId { get; set; }
    }
    #endregion

    public class AutoMockTestQuestionList : CurrentUser
    {
        public Guid MockTestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();

    }

    #region StudentDashboard MockTests
    public class MockTestInstitute : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        public Guid InstituteId { get; set; }
    }
    #endregion


}

