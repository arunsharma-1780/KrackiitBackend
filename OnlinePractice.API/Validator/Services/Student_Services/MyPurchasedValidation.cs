using OnlinePractice.API.Validator.Interfaces.Student_Interfaces;

namespace OnlinePractice.API.Validator.Services.Student_Services
{
    public class MyPurchasedValidation : IMyPurchasedValidation

    {
        public CreateMyPurchasedValidator CreateMyPurchasedValidator { get; set; } = new();
        public GetMyPurchasedMocktestValidator GetMyPurchasedMocktestValidator { get; set; } = new();
        public GetMyPurchasedEbooksValidator GetMyPurchasedEbooksValidator { get; set; } = new();
        public GetMyPurchasedVideosValidator GetMyPurchasedVideosValidator { get; set; } = new();
        public GetMyPurchasedPreviousYearPaperValidator GetMyPurchasedPreviousYearPaperValidator { get; set; } = new();
    }

    
}
