namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IMyCartValidation
    {
        public AddToCartValidator AddToCartValidator { get; set; }
        public RemoveItemFromMyCartValidator RemoveItemFromMyCartValidator { get; set; }
    }
}
