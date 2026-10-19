using AlfaHackaton.Result.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlfaHackaton.Controllers.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult Problem<T>(Result.Result<T> result)
    {
        var statusCode = result.Error.Type switch
        {
            ErrorType.NoRights => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.CorruptedInfo => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };
        if (statusCode == StatusCodes.Status400BadRequest && result.Content is not null)
        {
            return new ObjectResult(result.Content) { StatusCode = statusCode };
        }

        if (result.Error is null)
        {
            return StatusCode(statusCode);
        }

        return new ObjectResult(new { ErrorMessage = result.Error.Title }) { StatusCode = statusCode };
    }

    protected IActionResult OkOrProblem<T>(Result.Result<T> result)
    {
        if (result.Error is null)
        {
            return Ok(result.Content);
        }
        
        return Problem(result);
    }
}
