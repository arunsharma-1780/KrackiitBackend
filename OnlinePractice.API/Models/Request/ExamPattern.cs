using MimeKit.Cryptography;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    #region CreateExamPattern
    public class CreateExamPattern : CurrentUser
    {
        public string ExamPatternName { get; set; } = string.Empty;
        public List<Section> Section { get; set; } = new();
    }


    public class Section
    {
        public Guid SubjectId { get; set; }
        public List<Subsection> SubSection { get; set; } = new();
    }

    public class Subsection
    {
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
    }
    #endregion

    #region EditExamPattern
    public class EditExamPattern : CurrentUser
    {
        public Guid Id { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
        public List<EditSection> Section { get; set; } = new();
    }
    public class EditSection
    {
        public Guid SubjectId { get; set; }
        public List<EditExamPatternSection> SubSection { get; set; } = new();
    }
    public class EditExamPatternSection
    { 
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
    }

    public class EditGeneralInstruction : CurrentUser
    {
        public Guid Id { get; set; }
        public string GeneralInstruction { get; set; } = string.Empty;
    }
    public class CheckExamPatternName
    {
        public string ExamPatternName { get; set; } = string.Empty;
    }
    #endregion
    public class GetExamPatternId : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class GetAllExamPattern : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class GetByExamPatternId
    {
        public Guid Id { get; set; }
    }
    public class GetSectionList
    {
        public Guid ExamPatternId { get; set; }

        public Guid SubjectId { get; set; }
    }

    public class CheckSubject
    {
        public Guid SubjectId { get; set;}
    }
    public class CheckExamPatterNameAndId
    {
        public Guid Id { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
    }
}
