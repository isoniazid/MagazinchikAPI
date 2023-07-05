using System.Text.Json;
using FluentValidation;
using MagazinchikAPI.Infrastructure.ExceptionHandler;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }

        catch (APIException ex)
        {
            await HandleAPIMessageAsync(context, ex).ConfigureAwait(false);
        }

        catch(ValidatorException ex)
        {
            await HandleValidatorMessageAsync(context, ex).ConfigureAwait(false);
        }

        catch (NotImplementedException)
        {
            await HandleNotImplementedMessageAsync(context).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await HandleMessageAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static async Task HandleAPIMessageAsync(HttpContext context, APIException exception)
    // Ошибки сервера, связанные с API. Они вполне могут возникать
    {
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new APIErrorMessage(exception));

        context.Response.StatusCode = exception.StatusCode;
        await context.Response.WriteAsync(result);
    }

    private static async Task HandleValidatorMessageAsync(HttpContext context, ValidatorException exception)
    {
        context.Response.ContentType = "applicaiton/json";
        context.Response.StatusCode = exception.StatusCode; 

        
        var result = JsonSerializer.Serialize(new ValidatorErrorMessage(exception));

        await context.Response.WriteAsync(result);   
    }

    private static async Task HandleNotImplementedMessageAsync(HttpContext context)
    // Если чего-то нет, то это нормально, что оно не работает
    {
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new { statusCode = 501, message = "Эта функция пока не реализована"});

        context.Response.StatusCode = StatusCodes.Status501NotImplemented;
        
        await context.Response.WriteAsync(result);
    }

    private static async Task HandleMessageAsync(HttpContext context, Exception exception) 
    // Внутренние ошибки сервера. Их быть в норме не должно
    {
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new { statusCode = 500, message = exception.Message });

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        await context.Response.WriteAsync(result);
    }

}