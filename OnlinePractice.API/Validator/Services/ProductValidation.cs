using OnlinePractice.API.Validator.Interfaces;

namespace OnlinePractice.API.Validator.Services
{
    public class ProductValidation : IProductValidation
    {
        public CreateProductValidator CreateProductValidator { get; set; } = new();
        public EditProductValidator EditProductValidator { get; set; } = new();
        public ProductValidator ProductValidator { get; set; } = new();
        public DeleteProductValidator DeleteValidator { get; set; } = new();
    }
}
