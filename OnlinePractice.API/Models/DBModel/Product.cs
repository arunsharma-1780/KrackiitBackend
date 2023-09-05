using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{

    public class Product : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(150)")]
        public string ImageUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

}
