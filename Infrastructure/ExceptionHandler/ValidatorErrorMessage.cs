
    public class ValidatorErrorMessage
    {
        public int StatusCode {get; set;}
        public string Message {get; set;} = string.Empty;
        public Dictionary<string, string> Errors {get; set;} = new();

        public ValidatorErrorMessage(ValidatorException exception)
        {
            StatusCode = exception.StatusCode;
            Message = "validation_error";
            Errors = exception.ValidationErrors;

        } 
        
    }
