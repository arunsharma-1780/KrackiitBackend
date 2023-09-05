using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnlinePractice.API.Models.Request
{
    public class CreateProduct : CurrentUser
    {
        public string Name { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;
        //public string ImageUrl { get; set; } = string.Empty;
        //public double Price { get; set; }
        //public double Discount { get; set; }
        //public string Sku { get; set; } = string.Empty;
        //public string Status { get; set; } = string.Empty;
    }
    public class EditProduct : CreateProduct
    {
        public Guid Id { get; set; }
    }
    public class ProductById : CurrentUser
    {
        public Guid Id { get; set; }

    }
}
