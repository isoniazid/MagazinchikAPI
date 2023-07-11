namespace MagazinchikAPI.Infrastructure.ExceptionHandler
{
    public class APIErrorMessage
    {
        public APIErrorMessage(APIException ex)
        {
            StatusCode = ex.StatusCode;
            Message = ex.Message;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}