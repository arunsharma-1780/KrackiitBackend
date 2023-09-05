namespace OnlinePractice.API.Filters
{
    public class ResponseResult<T>
    {
        public ResponseResult(int responseCode, bool status, string message, T data)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Messages = message;
            Data = data;
        }
        public ResponseResult(int responseCode, bool status, string message)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Messages = message;
        }
        public int ResponseCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Messages { get; set; }
        public T? Data { get; set; }



        public static ResponseResult<T> Success(string message, T data)
        {
            return new ResponseResult<T>(200, true, message, data);
        }
        public static ResponseResult<T> Success(string message)
        {
            return new ResponseResult<T>(200, true, message);
        }
        public static ResponseResult<T> Failure(string message)
        {
            return new ResponseResult<T>(400, false, message);
        }
        public static ResponseResult<T> Failure(string message,T data)
        {
            return new ResponseResult<T>(400, false, message, data);
        }
        public static ResponseResult<T> Exception(string message)
        {
            return new ResponseResult<T>(500, false, message);
        }
    }
}
