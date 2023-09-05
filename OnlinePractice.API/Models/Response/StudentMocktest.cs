using OnlinePractice.API.Models.Enum;
using Stripe;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Response
{
    #region Student MockTest

    public class StudentMockTestList
    {
        public List<StudentMockTest> StudentMockTests { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class StudentMockTest
    {
        public Guid MockTestId { get; set; }
        public string MocktestName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public Guid SubTopicId { get; set; }
        public string SubTopicName { get; set; } = string.Empty;
        public string TestAvailability { get; set; } = string.Empty;
        public TimeSpan RemainingDuration { get; set; }
        public int RemainingAttempts { get; set; }
        public bool IsRetake { get; set; }
        public int TotalAttempts { get; set; }
        public int AlreadyResultGenerated { get; set; }
        public TimeSpan TotalGapBetweenReattempts { get; set; }
        public string Language { get; set; } = string.Empty;
        public TimeSpan MockTestDuration { get; set; }
        public DateTime? TestStartTime { get; set; }
        public DateTime? StartsInTime { get; set; }
        public DateTime? StartsInDays { get; set; }
        public bool IsDay { get; set; }
        public string Status { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsPurchased { get; set; }
    }
    public class CustomeStudentMockTestList
    {
        public List<CustomeStudentMockTest> StudentMockTests { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class CustomeStudentMockTest
    {
        public Guid MockTestId { get; set; }
        public string MocktestName { get; set; } = string.Empty;
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<string> Language { get; set; } = new();
        public bool IsRetake { get; set; }
        public TimeSpan RemainingDuration { get; set; }
        public int RemainingAttempts { get; set; }

        public TimeSpan MockTestDuration { get; set; }
        public DateTime? TestStartTime { get; set; }
        public int TotalQuestion { get; set; }
        public int Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCustome { get; set; }
    }

    public class StudentAutoMockTestQuestionList : CurrentUser
    {
        public Guid StudentMockTestId { get; set; }
        public Guid SubjectId { get; set; }
        public QuestionLevel QuestionLevel { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<MockTestQuestionss> MockTestQuestions { get; set; } = new();

    }

    #endregion

    #region Student Question Panel
    public class StudentMocktestPanel
    {

        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public string ExamPatternName { get; set; } = string.Empty;
        public bool IsRetake { get; set; }
     //   public bool IsPaused { get; set; }
        public int TotalAttempts { get; set; }
        
        public int RemainingAttempts { get; set; }
        public TimeSpan RemainingDuration { get; set; }
        public ResultDeclaration ResultDeclaration { get; set; }
        public bool IsShowCorrectAnswer { get; set; }
        public BackButton BackButton { get; set; } // 1:Allowed ,2:NotAllowed
        public string GeneralInstructions { get; set; } = string.Empty;
        public TimeSpan TimeDuration { get; set; }
        public DateTime? TestStartTime { get; set; }
        public List<string> Languages { get; set; } = new();
        public List<StudentMocktestPanelList> MocktestPanelList { get; set; } = new();

    }
    public class StudentMocktestPanelList
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<SubjectwiseSection> SubjectwiseSection { get; set; } = new();
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
        public int NoOfQue { get; set; }
        public int SectionCount { get; set; }
    }
    public class SubjectwiseSection
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalAttempt { get; set; }
        public int TotalQuestions { get; set; }
        public List<StudentMockTestQuestions> MockTestQuestions { get; set; } = new();

    }
    public class StudentMockTestQuestions
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public string QuestionLevel { get; set; } = string.Empty;//Easy=1,Medium=2,Hard=3
        public StudentMockQuestionTableDataList QuestionTableData { get; set; } = new();
        public int Mark { get; set; }
        public int NegativeMark { get; set; }

    }
    public class StudentMockQuestionTableDataList
    {
        public StudentMockEnglish? English { get; set; } = new();
        public StudentMockHindi? Hindi { get; set; } = new();
        public StudentMockGujarati? Gujarati { get; set; } = new();
        public StudentMockMarathi? Marathi { get; set; } = new();
    }
    public class StudentMockEnglish : StudentMockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class StudentMockHindi : StudentMockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class StudentMockGujarati : StudentMockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class StudentMockMarathi : StudentMockQuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class StudentMockQuestionCommon
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
    public class StudentAnswersPanelList
    {
        public List<StudentQuestionResponseV1> StudentQuestionResponse { get; set; } = new();
    }
    public class StudentQuestionResponse : CurrentUser
    {
        public Guid Id { get; set; }
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public Guid SubjectId { get; set; }
        public string StudentAnswer { get; set; } = string.Empty;
        public string CorrectAnswerA { get; set; } = string.Empty;
        public string CorrectAnswerB { get; set; } = string.Empty;
        public string CorrectAnswerC { get; set; } = string.Empty;
        public string CorrectAnswerD { get; set; } = string.Empty;
        public string Explaination { get; set; } = string.Empty;

        [DefaultValue(false)]
        public bool IsCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectD { get; set; }
        public bool IsMarkReview { get; set; }
        public bool IsAnswered { get; set; }
        [DefaultValue(false)]
        public bool IsShowResult { get; set; }

    }
    public class StudentQuestionResponseV1 : CurrentUser
    {
        public Guid Id { get; set; }
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string StudentAnswer { get; set; } = string.Empty;
        public string CorrectAnswerA { get; set; } = string.Empty;
        public string CorrectAnswerB { get; set; } = string.Empty;
        public string CorrectAnswerC { get; set; } = string.Empty;
        public string CorrectAnswerD { get; set; } = string.Empty;
        public string Explaination { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsActualCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectD { get; set; }


        [DefaultValue(false)]
        public bool IsCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectD { get; set; }
        public bool IsMarkReview { get; set; }
        public bool IsAnswered { get; set; }
        public bool IsVisited { get; set; }
        [DefaultValue(false)]
        public bool IsShowResult { get; set; }

    }
    public class StudentQuestionResponseV2 : CurrentUser
    {
        public Guid Id { get; set; }
        public Guid MockTestId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public Guid SubjectId { get; set; }
        public Guid SectionId { get; set; }
        public string StudentAnswer { get; set; } = string.Empty;
        public string CorrectAnswerA { get; set; } = string.Empty;
        public string CorrectAnswerB { get; set; } = string.Empty;
        public string CorrectAnswerC { get; set; } = string.Empty;
        public string CorrectAnswerD { get; set; } = string.Empty;
        public string Explaination { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsActualCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsActualCorrectD { get; set; }

        [DefaultValue(false)]
        public bool IsCorrectA { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectB { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectC { get; set; }
        [DefaultValue(false)]
        public bool IsCorrectD { get; set; }
        public bool IsMarkReview { get; set; }
        public bool IsAnswered { get; set; }
        public bool IsVisited { get; set; }
        [DefaultValue(false)]
        public bool IsShowResult { get; set; }

    }
    public class GeneralInstructions
    {
        public string GeneralInstruction { get; set; } = string.Empty;
    }
    public class StudentResultList
    {
        public List<StudentResults> StudentResults { get; set; } = new();
        public int TotalRecords { get; set; } 
    }
    public class StudentResults
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public double TotalMarks { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalQuestion { get; set; }
        public int OverAllCorrect { get; set; }
        public int OverAllInCorrect { get; set; }
        public double Score { get; set; }
        public string Result { get; set; } = string.Empty;
        public int OverAllSkipped { get; set; }
        public int Rank { get; set; }
        public List<SubjectWisePermormance> SubjectWisePermormance { get; set; } = new();
    }
    public class SubjectWisePermormance
    {
        public Guid SubjectId { get; set;}
        public string SubjectName { get; set; } = string.Empty;
        public int TotalSubjectQuestion { get; set; }
        public int Correct { get; set; }
        public int InCorrect { get; set; }
        public int Skipped { get; set; }
        public double TotalMarks { get; set; }
        public double Score { get; set; }

    }
    public class RestultStatus
    {
        public Guid Id { get; set; }
        public bool IsCompleted { get; set; }
    }
    #endregion

    #region Student Question Solution
    public class StudentQuestionSolution
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public List<StudentMocktestPanelListSolution> MocktestPanelList { get; set; } = new();

    }
    public class StudentMocktestPanelListSolution
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<SubjectwiseSectionSolution> SubjectwiseSection { get; set; } = new();
        public int NoOfQue { get; set; }
        public int SectionCount { get; set; }
    }
    public class SubjectwiseSectionSolution
    {
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public List<StudentMockTestQuestionsSolution> MockTestQuestions { get; set; } = new();

    }
    public class StudentMockTestQuestionsSolution
    {
        public string QuestionRefId { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public string QuestionLevel { get; set; } = string.Empty;//Easy=1,Medium=2,Hard=3
        public StudentMockQuestionTableDataListSolution QuestionTableData { get; set; } = new();

    }
    public class StudentMockQuestionTableDataListSolution
    {
        public StudentMockEnglishSolution? English { get; set; } = new();
        public StudentMockHindiSolution? Hindi { get; set; } = new();
        public StudentMockGujaratiSolution? Gujarati { get; set; } = new();
        public StudentMockMarathiSolution? Marathi { get; set; } = new();
    }
    public class StudentMockEnglishSolution : StudentMockQuestionCommonSolution
    {
        public Guid Id { get; set; }
    }
    public class StudentMockHindiSolution : StudentMockQuestionCommonSolution
    {
        public Guid Id { get; set; }
    }
    public class StudentMockGujaratiSolution : StudentMockQuestionCommonSolution
    {
        public Guid Id { get; set; }
    }
    public class StudentMockMarathiSolution : StudentMockQuestionCommonSolution
    {
        public Guid Id { get; set; }
    }
    public class StudentMockQuestionCommonSolution
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

    }
    #endregion
}
