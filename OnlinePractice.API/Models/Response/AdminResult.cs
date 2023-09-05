





namespace OnlinePractice.API.Models.Response
{
    public class AdminStudentResultList
    {
        public List<AdminStudentResults> StudentResults { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class AdminStudentResults
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public double TotalMarks { get; set; }
        public double TotalObtainMark { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public double Percentage { get; set; }
    }
    public class StudentResultDetail
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty;
        public MocktestResultDetails MocktestResultDetails { get; set; } = new();
        public ResultDetail ResultDetail { get; set; } = new();
        public SubjectDetail SubjectDetail { get; set; } = new();
    }
    public class MocktestResultDetails
    {
        public Guid Id { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public double TotalMarks { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public class ResultDetail
    {
        public int Rank { get; set; }
        public double TotalObtainMark { get; set; }
    }
    public class SubjectDetail
    {
        public List<SubjectResultDetail> SubjectResultDetails { get; set; } = new();
       public int TotalCorrect { get; set; }
        public int TotalIncorrect { get; set; }
        public int TotalSkipped { get; set; }

    }
    public class SubjectResultDetail
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int Correct { get; set; }
        public int InCorrect { get; set; }
        public int Skipped { get; set; }

    }


    public class ResultAnalysisList
    {
        public List<ResultAnalysis> ResultAnalyses { get; set; } = new();
        public int TotalRecord { get; set; }
    }
    public class ResultAnalysis
    {
        public Guid StudentId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int TotalMockTest { get; set; }
        public double TotalMarks { get; set; }
        public double TotalObtainedMarks { get; set; }
        public double AveragePercentage { get; set; }
    }

    public class ResultAnalysisDetail
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int TotalMockTest { get; set; }
        public double TotalMarks { get; set; }
        public double TotalObtainedMarks { get; set; }
        public double AveragePercentage { get; set; }

    }
    public class MockTestList
    {
        public List<MockTestInfoModel> MockTestInfoModels { get; set; } = new();
        public int TotalRecords { get; set; }
    }
    public class MockTestInfoModel
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
    }
}
