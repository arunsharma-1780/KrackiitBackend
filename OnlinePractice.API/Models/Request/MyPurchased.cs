using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{

    public class CreateMyPurchased : CurrentUser
    {
        public ProductCategory ProductCategory { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public Guid StudentId { get; set; } 
      
    }
    public class MyPurchasedMocktest : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
    public class MyPurchasedEbook : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
    public class MyPurchasedVideo : CurrentUser
    {

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
    public class MyPurchasedPreviousYearPAper : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
}
