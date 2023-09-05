namespace OnlinePractice.API.Models.Request
{
    public class CreateState:CurrentUser
    {
        public string StateName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
    }
    public class EditState : CreateState
    {
        public Guid Id { get; set; }
    }

    public class GetCountryId
    {
        public Guid CountryId { get; set; }
    }

    public class StateById : CurrentUser
    {
        public Guid Id { get; set; }

    }
}
