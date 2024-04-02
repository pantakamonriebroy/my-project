namespace ChillPay.Merchant.Register.Api.Models
{
    public class ApiResponseMessageModel : ApiResponseMessageModel<string>
    {
        public ApiResponseMessageModel(string content, ApiResponseStatus status, string message) : base(content, status, message)
        {
        }
    }

    public class ApiResponseMessageModel<T>
    {
        public ApiResponseMessageModel() { }

        public ApiResponseMessageModel(ApiResponseStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public ApiResponseMessageModel(T content, ApiResponseStatus status, string message)
        {
            Status = status;
            Message = message;
            Data = content;
        }

        public static ApiResponseMessageModel<T> Success(T content)
        {
            return new ApiResponseMessageModel<T>(content, ApiResponseStatus.Success, "Success");
        }

        public static ApiResponseMessageModel<T> Failed(string message = "Invalid Parameter")
        {
            return new ApiResponseMessageModel<T>(ApiResponseStatus.Fail, message);
        }

        public ApiResponseStatus Status { get; set; }
        public string Message { get; set; }
        public long TotalRecord { get; set; }
        public T Data { get; set; }

        public bool IsSuccessed => Status == ApiResponseStatus.Success;
    }
}
