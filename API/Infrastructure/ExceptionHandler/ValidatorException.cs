
public class ValidatorException : Exception
{
    public int StatusCode { get; set; }

    public Dictionary<string,string> ValidationErrors {get; set;} = new Dictionary<string,string>();

    public ValidatorException(FluentValidation.Results.ValidationResult validationResults)
    {
        StatusCode = StatusCodes.Status422UnprocessableEntity;
        foreach(var error in validationResults.Errors)
        {
            ValidationErrors.Add(error.PropertyName, error.ErrorMessage);
        }

    }
}

