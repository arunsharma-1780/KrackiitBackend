using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Request
{
    public class AddtoMyCart : CurrentUser
    {
        public ProductCategory ProductCategory { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
    }
    public class RemoveItemFromMyCart : CurrentUser
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
    }

    public class ProductIdCheck : CurrentUser
    {
        public Guid Id { get; set; }
    }
}
