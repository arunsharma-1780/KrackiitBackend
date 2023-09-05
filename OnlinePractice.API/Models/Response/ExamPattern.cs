namespace OnlinePractice.API.Models.Response
{
    public class ExamPattern : CurrentUser
    {
        public Guid Id { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
        public string GeneralInstruction { get; set; } = string.Empty;
        public List<Section> Section { get; set; } = new();
        public int TotalQuestion { get; set; }
    }
    public class Section
    {
        public Guid SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;
        public List<Subsection> SubSection { get; set; } = new();
        public int TotalSectionQuestions { get; set; }
    }

    public class Subsection
    {
        public Guid Id { get; set; }
        public string SectionName {get; set; } = string.Empty ;
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }
    }
    #region GetAllExamList
    public class ExamPatternList
    {
        public int TotalRecords { get; set; }
        public List<ExamPattern1> ExamPatterns { get; set; } = new();
    }
    public class ExamPattern1
    {
        public Guid Id { get; set; }
        public string ExamPatternName { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
    public class ExamPatternSubjectList
    {
        public List<ExamPatternSubjects> ExamPatternSubjects { get; set; } = new();
    }
    public class ExamPatternSubjects
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }
    public class ExamPatternSectionList
    {
        public List<ExamPatternSection> ExamPatternSections { get; set; } = new();
    }
    public class ExamPatternSection
    {
        public Guid Id { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TotalAttempt { get; set; }

    }
    #endregion
}
