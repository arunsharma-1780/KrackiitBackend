namespace OnlinePractice.API.Models.Request
{
    public class CreateCountries:CurrentUser
    {
        public string CountryName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
    public class EditCountries : CreateCountries
    {
        public Guid id { get; set; }
    }
    public class CountryById : CurrentUser
    {
        public Guid Id { get; set; }

    }
}
