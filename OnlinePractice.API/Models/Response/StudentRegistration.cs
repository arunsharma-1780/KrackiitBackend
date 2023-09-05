using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Response
{
    public class StudentList
    {
        public List<StudentDetails> StudentDetails { get; set; } = new(); 
        public int TotalRecords { get; set; }
    }
    public class StudentDetails
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty;
        public DateTime? EnrollmentDate { get; set; } 
        public string InstituteName { get; set; } = string.Empty;       
    }
    public class StudentByIdInfo
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfileImage { get; set; }
        public string InstituteId { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string InstituteLogo { get; set; } = string.Empty;
        public string SubcourseId { get; set; } = string.Empty;
        public string SubcourseName { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public double Balance { get; set; }
        public int TotalItems { get; set; }
        public List<PurchaseItems> PurchaseItems { get; set; } = new();
        public List<WalletTransaction> WalletTransactions { get; set; } = new();

    }
    public class CustomerBulkUplpad
    {
        public int TotalCustomer { get; set; } = 0;
        public int TotalAdded { get; set; } = 0;
        public int TotalNotAdded { get; set; } = 0;
    }

    public class PurchaseItems
    {
        public string ItemName { get; set; } = string.Empty;
        public List<string> Type { get; set; } = new();
        public DateTime? Date { get; set; }
        public double Amount { get; set; }

    }

    public class WalletTransaction
    {
        public double Amount { get; set; }
        public DateTime? Date { get; set; }
        public List<string> Type { get; set; } = new();
    }

}
