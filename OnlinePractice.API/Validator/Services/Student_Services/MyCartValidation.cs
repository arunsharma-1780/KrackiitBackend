using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class MyCartValidation : IMyCartValidation
    {
        public AddToCartValidator AddToCartValidator { get; set; } = new();
        public RemoveItemFromMyCartValidator RemoveItemFromMyCartValidator { get; set; } = new();
    }
}
