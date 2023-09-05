using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class CreateStudentWallet : CurrentUser
    {
        public string OrderId { get; set; } =string.Empty;
        public double Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class CreateWalletHistory
    {
        public Guid ProductId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public double NewAmountAdded { get; set; }
        public double UsedAmount { get; set; }
    }

    public class PurchaseFromWallet : CurrentUser
    {
        public double Amount { get; set; }
    }
    public class Checkout : CurrentUser
    {
        public double TotalAmount { get; set; }

        public List<CheckOutItems> CheckOutItems { get; set; } = new();
    }

    public class CheckOutItems
    {
        public ProductCategory ProductCategory { get; set;}
        public Guid ProductId { get; set;}
        public double Price { get; set;}
    }
    public class GetWalletHistory : CurrentUser
    {
            [DefaultValue(1)]
            public int PageNumber { get; set; }
            [DefaultValue(10)]
            public int PageSize { get; set; }
    }
}
