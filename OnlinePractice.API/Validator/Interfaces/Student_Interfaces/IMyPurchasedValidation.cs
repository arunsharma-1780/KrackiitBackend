namespace OnlinePractice.API.Validator.Interfaces.Student_Interfaces
{
    public interface IMyPurchasedValidation

    {
        public CreateMyPurchasedValidator CreateMyPurchasedValidator { get; set; }
        public GetMyPurchasedMocktestValidator GetMyPurchasedMocktestValidator { get; set; }
        public GetMyPurchasedEbooksValidator GetMyPurchasedEbooksValidator { get; set; }
        public GetMyPurchasedVideosValidator GetMyPurchasedVideosValidator { get; set; }

        public GetMyPurchasedPreviousYearPaperValidator GetMyPurchasedPreviousYearPaperValidator { get; set; }


    }
}
