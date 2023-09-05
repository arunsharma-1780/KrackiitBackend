using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Response
{
    public class StudentWallet
    {
        public Guid StudentId { get; set; }
        public double Balance { get; set; }
        public List<StudentWalletHistory> StudentWalletHistory { get; set; } = new();
        public int TotalRecords { get; set; }
    }

    public class StudentWalletHistory
    {
        public double CreditAmount { get; set; }
        public double DebitAmount { get; set; }
        public DateTime? CreationDate { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; }

    }

    public class StudentBalance
    {
        public double Balance { get; set; }
    }
}
