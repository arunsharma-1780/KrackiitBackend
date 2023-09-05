namespace OnlinePractice.API.Models.Response
{
    public class Countries
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
