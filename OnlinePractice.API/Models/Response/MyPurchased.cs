using OnlinePractice.API.Models.Enum;
using System.Drawing;

namespace OnlinePractice.API.Models.Response
{
    #region Module list
    public class StudentModulesList
    {
        public List<StudentModules> studentModules { get; set; } = new();
    }
    public class StudentModules
    {
        public string ModuleCategoryName { get; set; } = string.Empty;
    }

    #endregion

    #region MockTest
    public class MyPurchasedMockTestsLists
    {
        public int TotalRecords { get; set; }
        public List<MyPurchasedMockTests> MyPurchasedMockTests { get; set; } = new();
    }

    public class MyPurchasedMockTests
    {
        public Guid MockTestId { get; set; }
        public string MocktestName { get; set; } = string.Empty;
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
        public DateTime? PurchaseDate { get; set;}

    }
    #endregion

    #region Ebooks
    public class MyPurchasedEbooksLists
    {
        public int TotalRecords { get; set; }
        public List<MyPurchasedEbooks> MyPurchasedEbooks { get; set; } = new();
    }

    public class MyPurchasedEbooks
    {
        public Guid EbookId { get; set; }
        public string EbookTitle { get; set; } = string.Empty;
        public string EbookPdfURL { get; set; } = string.Empty;
        public string EbookThumbnail { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public double Price { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public DateTime? PurchaseDate { get; set; }

    }
    #endregion

    #region Videos
    public class MyPurchasedVideosLists
    {
        public int TotalRecords { get; set; }
        public List<MyPurchasedVideos> MyPurchasedVideos { get; set; } = new();
    }

    public class MyPurchasedVideos
    {
        public Guid VideoId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public string VideoThumbnail { get; set; } = string.Empty;
        public string VideoURL { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public double Price { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public DateTime? PurchaseDate { get; set; }
    }
    #endregion

    #region PreviousYearPaper
    public class MyPurchasedPYPLists
    {
        public int TotalRecords { get; set; }
        public List<MyPurchasedPYP>  MyPurchasedPYPs { get; set; } = new();
    }

    public class MyPurchasedPYP
    {
        public Guid PaperId { get; set; }
        public string PaperTitle { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Language { get; set; } = string.Empty;
        public string PaperPdfUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
    }
    #endregion


}


