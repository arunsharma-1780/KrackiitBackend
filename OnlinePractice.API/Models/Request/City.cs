namespace OnlinePractice.API.Models.Request
{
    public class CreateCity:CurrentUser
    {
        public string CityName { get; set; } = string.Empty;
        public string AliasName { get; set; } = string.Empty;
        public Guid StateId { get; set; }
    }

    public class EditCity:CreateCity
    {
        public Guid Id { get; set; }
    }

    public class CityById:CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class GetStateId
    {
        public Guid StateId { get; set; }
    }
}
