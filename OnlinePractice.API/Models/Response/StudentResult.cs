namespace OnlinePractice.API.Models.Response
{
    public class StudentResultAnalysis
    {
        public string Name { get; set; } = string.Empty;
        public string InstituteCode { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int TotalMockTest { get; set; }
        public double TotalMarks { get; set; }
        public double TotalObtainedMarks { get; set; }
        public double AveragePercentage { get; set; }
    }

    public class StudentMockTestWiseResultAnalysis
    {
        public string Name { get; set; } = string.Empty;
        public int Rank { get; set; }
        public double TotalMarks { get; set; }
        public double TotalObtainedMarks { get; set; }
        public double AveragePercentage { get; set; }
    }

    public class ExistingMockTestList
    {
        public List<ExistingMockTestDetails> ExistingMockTestDetails { get; set; } = new();
        public int TotalRecords { get; set; }
    }

    public class ExistingMockTestDetails
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
    }

    public class MocktestDataResultList
    {
        public List<MockTestDetails> MockTestDetails { get; set; } = new();
        public int TotalRecords { get; set;}
    }
    public class MockTestDetails
    {
        public Guid MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string SubTopicName { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Language { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsCustom { get; set; }

    }
}
