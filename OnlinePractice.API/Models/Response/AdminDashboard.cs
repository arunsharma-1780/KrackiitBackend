using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Response
{
    public class TotalSaleDetails
    {
        public double Sale { get; set; }
        public double TotalSale { get; set; }

    }
    public class TotalEnrollmentDetails
    {
        public double Enrollment { get; set; }
        public double TotalEnrollment { get; set; }
    }

    public class InstituteStudent
    {
        public List<InstituteStudentCourse> InstituteStudentCourses { get; set; } = new();

        public int TotalRecords { get; set; }
        //public int TotalActiveStudent { get; set; }
    }
    public class InstituteStudentCourse
    {
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }
    public class RecentTransactionandTimeLineList
    {
        public List<RecentTransactions> RecentTransactions { get; set; } = new();
        public List<TimeLine> TimeLines { get; set; } = new();

    }
    public class RecentTransactions
    {
        public Guid OrderId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public double Amount { get; set; }
    }
    public class TimeLineList
    {
        public List<TimeLine> TimeLines { get; set; } = new();
    }
    public class TimeLine
    {
        public Guid Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public List<string> Type { get; set; } = new();
    }

}
