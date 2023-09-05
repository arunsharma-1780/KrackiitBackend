namespace OnlinePractice.API.Models.Response
{
    public class City
    {
        public Guid Id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid StateId { get; set; }
    }
}
