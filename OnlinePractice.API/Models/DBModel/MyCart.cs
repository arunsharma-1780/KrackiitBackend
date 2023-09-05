using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class MyCart :BaseModel
    {
        public ProductCategory ProductCategory { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public Guid StudentId { get; set; }
        public bool IsPurchased { get; set; }

    }
}

