using FluentValidation;
using Req = OnlinePractice.API.Models.Request;
using System.Text.RegularExpressions;
using OnlinePractice.API.Models.Request;

namespace OnlinePractice.API.Validator
{
    public class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
        {
            //RuleFor(prop => prop.Name).NotEmpty().NotNull().WithMessage("Product Name is required!").MaximumLength(100).WithMessage("Product Name must be less than 100 characters!").Matches("^([a-zA-Z0-9]+\\s)*[a-zA-Z0-9]+$").WithMessage("Product Name is invalid! it will allow alphanumeric only!");
            //RuleFor(prop => prop.Description).MaximumLength(500).WithMessage("Description must be less than 500 characters!");
            //When(prop => !string.IsNullOrEmpty(prop.ImageUrl), () =>
            //{
            //    RuleFor(prop => prop.ImageUrl).Matches(@"([0-9a-zA-Z :\\-_!@$%^&*()])+(.jpg|.jpeg|.bmp|.gif|.png)$", RegexOptions.IgnoreCase).WithMessage("Image path not valid!");
            //});
            //RuleFor(prop => prop.Price).NotEmpty().NotNull().WithMessage(" Product price is required!");
            //RuleFor(prop => prop.Discount).NotEmpty().NotNull().WithMessage("discount is required!");
            //RuleFor(prop => prop.Sku).NotEmpty().NotNull().WithMessage("SKU is required!");
            //RuleFor(prop => prop.Status).NotEmpty().WithMessage("Status must not be empty").NotNull().WithMessage("Status is required!");

        }
    }
    public class EditProductValidator : AbstractValidator<EditProduct>
    {
        public EditProductValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
            RuleFor(prop => prop).SetValidator(new CreateProductValidator());
        }
    }
    public class ProductValidator : AbstractValidator<ProductById>
    {
        public ProductValidator()
        {
            RuleFor(prop => prop.Id).NotEmpty().NotNull().WithMessage("Id is required!");
        }
    }
    public class DeleteProductValidator : AbstractValidator<ProductById>
    {
        public DeleteProductValidator()
        {
            RuleFor(prop => prop).SetValidator(new ProductValidator());
        }
    }

}

