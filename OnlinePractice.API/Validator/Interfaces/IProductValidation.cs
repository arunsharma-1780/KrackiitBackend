namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IProductValidation
    {
        public CreateProductValidator CreateProductValidator { get; set; }
        public EditProductValidator EditProductValidator { get; set; }
        public ProductValidator ProductValidator { get; set; }
        public DeleteProductValidator DeleteValidator { get; set; }

    }
}
