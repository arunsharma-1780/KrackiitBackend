namespace OnlinePractice.API.Models.Response
{
    public class UserById
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? Location { get; set; }
        public string? ProfileImage {  get; set; }
    }
    public class UserDetailsById
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public List<string> SecondaryEmail { get; set; } = new();
        public string? MobileNumber { get; set; }
        public string? Location { get; set; }
        public string? ProfileImage { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class UserDetails
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }

    }

    public class MobileNumbers
    {
        public string? MobileNumber { get; set; }
    }


}
