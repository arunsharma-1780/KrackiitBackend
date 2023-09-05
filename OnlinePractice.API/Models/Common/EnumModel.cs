namespace OnlinePractice.API.Models.Common
{
    public class EnumModel
    {
        public int Value { get; set; }
        public string? Name { get; set; }
    }

    public class ResultMessage
    {
        public string Message { get; set; } =string.Empty;
        public bool Result { get; set; }
        public string Token { get; set; } = string.Empty;
    }
    public class ResultMessageAdmin
    {
        public string Message { get; set; } = string.Empty;
        public bool Result { get; set; }
    }
    public class ResultDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Result { get; set; }
    }
}
