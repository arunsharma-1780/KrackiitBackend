using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Request
{

    public class FilterModel
    {
        public DurationFilter DurationFilter { get; set; }
    }
    public class TotalSaleDetails
    {
        public double TodaySale { get; set; }
        public double TotalSale { get; set; }

    }
}
