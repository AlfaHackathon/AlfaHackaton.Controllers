using AlfaHackaton.Auth.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AlfaHackaton.Controllers.Middlewares;

public sealed class AuthExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await ExceptionHandling(context, e);
        }
    }

    private async Task ExceptionHandling(HttpContext context, Exception e)
    {
        context.Response.StatusCode = e switch
        {
            TokenInvalidException => StatusCodes.Status401Unauthorized,
        };

        await context.Response.WriteAsJsonAsync(new { ErrorMessage = e.Message });
    }
}