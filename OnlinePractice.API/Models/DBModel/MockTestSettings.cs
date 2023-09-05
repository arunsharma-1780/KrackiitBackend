using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class MockTestSettings : BaseModel
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
        public TimeSpan TimeSettingDuration { get; set; }
        public TestAvailability TestAvailability { get; set; }//Specific = 1,Always = 2
        public DateTime? TestStartTime { get; set; }
        public DateTime? TestSpecificToDate { get; set; }
        public DateTime? TestSpecificFromDate { get; set; }
        #endregion

        #region AttemptSettings
        public bool IsAllowReattempts { get; set; }
        //public bool IsUnlimitedAttempts { get; set; }
        public int TotalAttempts { get; set; }
        public int ReattemptsDays { get; set; }
        public TimeSpan ReattemptsDuration { get; set; }
        #endregion

        #region ResumeSettings
        public bool IsTestResume { get; set; }
        //public bool IsUnlimitedResume { get; set; }
        //public int TotalResume { get; set; }
        #endregion

        #region Appearance
       // public BackButton BackButton { get; set; } // 1:Allowed ,2:NotAllowed
        #endregion

        #region ResultDeclarationOption
        public bool IsMarksResultFormat { get; set; }
        public bool IsPassFailResultFormat { get; set; }
       // public bool IsRankResultFormat { get; set; }
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
        public Guid ExamTypeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid ExamPatternId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public Guid SectionId { get; set; }
        public QuestionType QuestionType { get; set; }

    }

}
