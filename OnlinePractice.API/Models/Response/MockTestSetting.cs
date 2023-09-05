using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Request;
using System.Globalization;

namespace OnlinePractice.API.Models.Response
{
    public class MockTestSetting
    {
        #region BasicDetails
        public string MockTestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        #endregion

        #region Pricing
        public bool IsFree { get; set; }
        public double Price { get; set; }
        #endregion

        #region TimeSettings
        public int TimeDurationHours { get; set; }
        public int TimeDurationMinutes { get; set; }
        public TimeSpan TimeSettingDuration { get; set; }
        public DateTime? TestStartTime { get; set; }
        public TestAvailability TestAvailability { get; set; }//Specific = 1,Always = 2
        public DateTime? TestSpecificToDate { get; set; }
        public DateTime? TestSpecificFromDate { get; set; }
        #endregion

        #region AttemptSettings
        public bool IsAllowReattempts { get; set; }
        public bool IsUnlimitedAttempts { get; set; }
        public int TotalAttempts { get; set; }
        public int ReattemptsDays { get; set; }
        public int ReattemptsHours { get; set; }
        public int ReattemptsMinutes { get; set; }
        public TimeSpan ReattemptsDuration { get; set; }
        #endregion

        #region ResumeSettings
        public bool IsTestResume { get; set; }
        //public bool IsUnlimitedResume { get; set; }
        //public int TotalResume { get; set; }
        #endregion

        #region Appearance
        public BackButton BackButton { get; set; } // 1:Allowed ,2:NotAllowed
        #endregion

        #region ResultDeclarationOption
        public bool IsMarksResultFormat { get; set; }
        public bool IsPassFailResultFormat { get; set; }
        public bool IsRankResultFormat { get; set; }

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

    public class MockTestInfo
    {
        public Guid Id { get; set; }
        public string Langauge { get; set; } = string.Empty;
    }
    public class Languages
    {
        public string Language { get; set; } = string.Empty;
    }

    public class MockTestSettingList
    {
        public int TotalRecords { get; set; }
        public List<GetAllMocktest> GetAllMocktests { get; set; } = new();
    }
    public class MockTestSettingListV1
    {
        public int TotalRecords { get; set; }
        public List<GetAllMocktestV1> GetAllMocktests { get; set; } = new();
    }

    public class GetAllMocktest
    {
        public string MockTestName { get; set; } = string.Empty;
        public string AddedBy { get; set; } = string.Empty;
        public string languages { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }
        public Guid InstituteId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
    public class GetAllMocktestV1
    {
        public Guid Id { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public string AddedBy { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public Guid? CreationUserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new();
        public List<string> MockTestType { get; set; } = new();
    }
    public class MockTestQuestionsList
    {
        public int TotalRecords { get; set; }
        public List<MockTestQuestions> MockTestQuestions { get; set; } = new();
    }

    public class MockTestQuestions
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public string QuestionLevel { get; set; } = string.Empty;//Easy=1,Medium=2,Hard=3
        public MockQuestionTableDataList QuestionTableData { get; set; } = new();
        public int Mark { get; set; }
        public int NegativeMark { get; set; }

    }
    public class MockQuestionTableDataList
    {
        public MockEnglish? English { get; set; } = new();
        public MockHindi? Hindi { get; set; } = new();
        public MockGujarati? Gujarati { get; set; } = new();
        public MockMarathi? Marathi { get; set; } = new();
    }
    public class MockEnglish : MockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class MockHindi : MockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class MockGujarati : MockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class MockMarathi : MockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class MockQuestionCommon
    {
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public bool IsCorrectA { get; set; }
        public string OptionB { get; set; } = string.Empty;
        public bool IsCorrectB { get; set; }
        public string OptionC { get; set; } = string.Empty;
        public bool IsCorrectC { get; set; }
        public string OptionD { get; set; } = string.Empty;
        public bool IsCorrectD { get; set; }
        public string Explanation { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

    }
    public class QuestionTableDataList
    {
        public English? English { get; set; } = new();
        public Hindi? Hindi { get; set; } = new();
        public Gujarati? Gujarati { get; set; } = new();
        public Marathi? Marathi { get; set; } = new();
    }
    public class OptionList
    {
        public string OptionA { get; set; } = string.Empty;
        public bool IsCorrectA { get; set; }
        public string OptionB { get; set; } = string.Empty;
        public bool IsCorrectB { get; set; }
        public string OptionC { get; set; } = string.Empty;
        public bool IsCorrectC { get; set; }
        public string OptionD { get; set; } = string.Empty;
        public bool IsCorrectD { get; set; }
    }
    #region AutomaticMockTest
    public class AutomaticMockTest
    {
        public Guid Id { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public List<SectionList> SectionList { get; set; } = new();
    }

    public class AutoMockTestQuestionListV1 : CurrentUser
    {

    }
    public class AutoMockTestQuestionList : CurrentUser
    {
        public Guid MockTestSettingId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public string Language { get; set; } = string.Empty;

        public bool IsDraft { get; set; }
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();

    }
    public class MockTestQuestionListPdf
    {
        public Guid MockTestSettingId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
        public string GeneralInstructions { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public List<MockTestQuestionssPdf> MockTestQuestions { get; set; } = new();
        public Guid SubjectId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public Guid SectionId { get; set; }
        public bool IsDraft { get; set; }
        public QuestionType QuestionType { get; set; }
        public DateTime? TestDate { get; set; }
        public int TimeDurationHours { get; set; }
        public int TimeDurationMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int TotalQuestions { get; set; }

    }
    public class MockTestQuestionssPdf
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<SectionDetailsPdf> SectionDetails { get; set; } = new();
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
        public int NoOfQue { get; set; }
        public int SectionCount { get; set; }

        public int SubjectTotalMarks { get; set; }
    }
    public class SectionDetailsPdf
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalAttempt { get; set; }
        public int TotalQuestions { get; set; }
        public int SectionTotalMark { get; set; }
        public List<MockTestQuestions> MockTestQuestions { get; set; } = new();

    }
    public class MockTestQuestionList
    {
        public Guid MockTestSettingId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
        public string GeneralInstructions { get; set; } = string.Empty;
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();
        public Guid SubjectId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public Guid SectionId { get; set; }
        public bool IsDraft { get; set; }
        public QuestionType QuestionType { get; set; }

    }

    public class MockTestQuestionss
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<SectionDetails> SectionDetails { get; set; } = new();
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
        public int NoOfQue { get; set; }
        public int SectionCount { get; set; }
    }

    public class SectionDetails
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalAttempt { get; set; }
        public int TotalQuestions { get; set; }
        public List<MockTestQuestions> MockTestQuestions { get; set; } = new();

    }

    public class SectionList
    {
        public Guid SubjectID { get; set; }
        public List<SubSection> subSection { get; set; } = new();
    }

    public class SubSection
    {
        public Guid SectionId { get; set; }
        public QuestionType QuestionType { get; set; }
    }

    #endregion

    #region Student Dashboard MockTest
    public class InstituteMockTestList
    {
        public List<InstituteMockTest> InstituteMockTest { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class InstituteMockTest
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public string TestAvailaiblityType { get; set; } = string.Empty;
        public string Description { get; set;} = string.Empty;
        public DateTime? StartsInTime { get; set; }
        public DateTime? StartsInDays { get; set; }
        public TimeSpan MockTestDuration { get; set; }
        public bool IsDay { get; set; }
        public string Status { get; set; } = string.Empty;

    }
    #endregion


}
