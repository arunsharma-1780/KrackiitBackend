namespace OnlinePractice.API.Models.Response
{
    public class AdminWalletList
    {
        public List<AdminWallet> AdminWallets { get; set;} = new();
        public int TotalRecords { get; set;}
        public double TotalAmount { get; set;}
    }
    public class AdminWallet
    {
        public Guid OrderId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public int TotalItem { get; set; }
        public double Amount { get; set; }
        public DateTime? OrderDate {
            get; set;
        }
    }

    public class TransactionDetails
    {
        public Guid OrderId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public int TotalItem { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderSummary> OrderSummaries { get; set; } = new();
        public DateTime? OrderDate
        {
            get; set;
        }
    }

    public class OrderSummary
    {
        public string ItemName { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
