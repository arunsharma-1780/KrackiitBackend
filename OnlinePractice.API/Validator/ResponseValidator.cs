using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Validator
{
    public class ValidationResult
    {
        [Required]
        public int ErrorCode { get; set; }
        [Required]
        public string PropertyName { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
    }
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Data = new List<ValidationResult>();
            Message = "";
            ErrorCode = 0;
        }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public List<ValidationResult> Data { get; set; }
    }

}
