namespace OnlinePractice.API.Models.Response
{
    public class State
    {
        public Guid Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
    }
}
