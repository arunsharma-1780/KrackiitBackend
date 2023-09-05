namespace OnlinePractice.API.Models.Common
{
    public class Result<T>
    {
        public Result(int responseCode, bool status, string message, T data)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Message = message;
            Data = data;
        }
        public Result(int responseCode, bool status, string message)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Message = message;
        }
        public int ResponseCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public static Result<T> Success(string message, T data)
        {
            return new Result<T>(0, true, message, data);
        }
        public static Result<T> Success(string message)
        {
            return new Result<T>(0, true, message);
        }
        public static Result<T> Failure(string message)
        {
            return new Result<T>(199, false, message);
        }
    }
}
