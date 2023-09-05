namespace OnlinePractice.API.Models.DBModel
{
    public class Token :BaseModel
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Tokens { get; set; } = string.Empty;

    }
}
