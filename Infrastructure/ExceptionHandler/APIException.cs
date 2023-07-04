public class APIException : Exception
{
    public int StatusCode { get; set; }

    public APIException(string message, int StatusCode) : base(message)
    {
        this.StatusCode = StatusCode;
    }
}