namespace OnlinePractice.API.Models.Enum
{
    public enum StatusFilter
    {
        All = 0,
        NotVisted = 1,
        InProgress = 2,
        Completed = 3,
        Expired = 4,

    }
    public enum CustomeStatusFilter
    {
        All = 0,
        NotVisted = 1,
        InProgress = 2,
        Completed = 3,
    }

    public enum PricingFilter
    {
        All = 0,
        Free = 1,
        Premium = 2
    }

    public enum PriceWiseSort
    {
        All = 0,
        HighToLow = 1,
        LowToHigh = 2
    }
    public enum LanguageFilter
    {
        All = 0,
        English = 1,
        Hindi = 2,
        Gujarati = 3,
        Marathi = 4
    }
}
