using OnlinePractice.API.Models.Enum;
using System.Globalization;

namespace OnlinePractice.API.Models.Response
{
    public class QuestionBank
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }

        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public string QuestionLevel { get; set; } = string.Empty;//Easy=1,Medium=2,Hard=3
        public QuestionTableData QuestionTableData { get; set; } = new();
        public int Mark { get; set; }
        public int NegativeMark { get; set; }
        public bool IsPartiallyCorrect { get; set; }
        public int PartialThreeCorrectMark { get; set; } = 3;
        public int PartialTwoCorrectMark { get; set; } = 2;
        public int PartialOneCorrectMark { get; set; } = 1;

    }
    public class QuestionTableData
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public English English { get; set; } = new();
        public Hindi? Hindi { get; set; } = new();
        public Gujarati? Gujarati { get; set; } = new();
        public Marathi? Marathi { get; set; } = new();
    }

    public class English : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class Hindi : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class Gujarati : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class Marathi : QuestionCommon
    {
        public Guid Id { get; set; }
    }

    public class QuestionCommon
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
    }
    public class QuestionBankList
    {
        public List<AllQuestionBank> QuestionBanks { get; set; } = new();
        public int TotalRecords { get; set; }

    }
    public class AllQuestionBank
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string QuestionLevel { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public Guid? CreatorUserId { get; set; }
        public DateTime? CreationDate { get; set; }
    }
    public class AllQuestionBankV1
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string QuestionLevel { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
    }
    public class TypesofId
    {
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
    }
}
