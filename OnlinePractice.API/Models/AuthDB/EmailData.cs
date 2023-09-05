namespace OnlinePractice.API.Models.AuthDB
{
    public class EmailData
    {
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<IFormFile> Attachments { get; set; } = new();
    }
}
