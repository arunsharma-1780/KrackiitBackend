using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{

    #region QuestinModels
    public class CreateQuestionBank : CurrentUser
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public QuestionLevel QuestionLevel { get; set; } //Easy=1,Medium=2,Hard=3
        public QuestionTableData QuestionTableData { get; set; } = new();
        public int Mark { get; set; }
        public int NegativeMark { get; set; }
        [DefaultValue(false)]
        public bool IsPartiallyCorrect { get; set; }
        public int PartialThreeCorrectMark { get; set; } = 3;
        public int PartialTwoCorrectMark { get; set; } = 2;
        public int PartialOneCorrectMark { get; set; } = 1;

    }
    public class GetAllQuestion
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }
        public QuestionLevel QuestionLevel{ get; set; }
        public Guid CreatorUserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetAll50Question
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class QuestionTableData
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public English English { get; set; } = new();
        public Hindi Hindi { get; set; } = new();
        public Gujarati Gujarati { get; set; } = new();
        public Marathi Marathi { get; set; } = new();
    }

    public class English : QuestionCommon
    {

    }
    public class Hindi : QuestionCommon
    {

    }
    public class Gujarati : QuestionCommon
    {

    }
    public class Marathi : QuestionCommon
    {

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

    #endregion
    public class GetQuestionBank : CurrentUser
    {
        public string QuestionRefId { get; set; } = string.Empty;

    }

    public class QuestionBankRefId : CurrentUser
    {
        public string QuestionRefId { get; set; } = string.Empty;
    }

    public class QuestionImage
    {
        public IFormFile? Image { get; set; }
    }

    public class QuestionBankFilter
    {
        public Guid SubCourseId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; } = Guid.Empty;
    }

    public class CheckSubjectCategoryId
    {
        public Guid Id { get; set; }
    }

    public class CheckTopicId
    {
        public Guid Id { get; set; }
    }

    public class CheckSubtopicId
    {
        public Guid Id { get; set; }
    }

    public class CheckReference
    {
        public String ReferenceId { get; set; } = string.Empty;
    }

    public class CheckUserExist
    {
        public Guid Id { get; set; }
    }

    public class EditQuestionBank :CurrentUser
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public QuestionLevel QuestionLevel { get; set; } //Easy=1,Medium=2,Hard=3
        public EditQuestionTableData QuestionTableData { get; set; } = new();
        public int Mark { get; set; }
        public int NegativeMark { get; set; }
        public bool IsPartiallyCorrect { get; set; }
        public int PartialThreeCorrectMark { get; set; } = 3;
        public int PartialTwoCorrectMark { get; set; } = 2;
        public int PartialOneCorrectMark { get; set; } = 1;

    }
    public class EditQuestionTableData
    {
        public string QuestionRefId { get; set; } = string.Empty;
        public EnglishEdit English { get; set; } = new();
        public HindiEdit Hindi { get; set; } = new();
        public GujaratiEdit Gujarati { get; set; } = new();
        public MarathiEdit Marathi { get; set; } = new();
    }

    public class EnglishEdit : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class HindiEdit : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class GujaratiEdit : QuestionCommon
    {
        public Guid Id { get; set; }
    }
    public class MarathiEdit : QuestionCommon
    {
        public Guid Id { get; set; }
    }

}
