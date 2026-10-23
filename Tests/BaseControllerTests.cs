using System.Net;
using AlfaHackaton.Controllers.Controllers;
using AlfaHackaton.Result;
using AlfaHackaton.Result.Errors;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public sealed class BaseControllerTests
{
    private sealed class Controller : BaseController
    {
        public IActionResult Respond<T>(Result<T> result) => OkOrProblem(result);
        public IActionResult RespondProblem<T>(Result<T> result) => Problem(result);
    }

    [Theory]
    [InlineData(409, ErrorType.CorruptedInfo)]
    [InlineData(422, ErrorType.CorruptedInfo)]
    [InlineData(503, ErrorType.NotFound)]
    [InlineData(403, ErrorType.Undefined)]
    public void ExplicitHttpStatusTakesPrecedence(int status, ErrorType type)
    {
        var response = Assert.IsType<ObjectResult>(new Controller().Respond(new Result<object>(
            new Error("Package error", ErrorSource.Client, type) { HttpStatusCode = (HttpStatusCode)status })));
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("Package error", response.Value!.GetType().GetProperty("ErrorMessage")!.GetValue(response.Value));
    }

    [Theory]
    [InlineData(ErrorType.CorruptedInfo, 400)]
    [InlineData(ErrorType.Unauthorized, 401)]
    [InlineData(ErrorType.NoRights, 403)]
    [InlineData(ErrorType.NotFound, 404)]
    [InlineData(ErrorType.Undefined, 500)]
    public void MissingHttpStatusRetainsTypeMapping(ErrorType type, int status)
    {
        var response = Assert.IsType<ObjectResult>(new Controller().Respond(new Result<object>(new Error("Failure", type: type))));
        Assert.Equal(status, response.StatusCode);
    }

    [Fact]
    public void SuccessAndBadRequestContentRetainTheirBodies()
    {
        var content = new { Name = "payload" };
        Assert.Same(content, Assert.IsType<OkObjectResult>(new Controller().Respond(new Result<object>(content))).Value);
        var badRequest = Assert.IsType<ObjectResult>(new Controller().Respond(new Result<object>(content,
            new Error("Validation", type: ErrorType.CorruptedInfo))));
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Same(content, badRequest.Value);
    }

    [Fact]
    public void ProblemWithoutErrorReturnsInternalServerError()
    {
        Assert.Equal(500, Assert.IsType<StatusCodeResult>(new Controller().RespondProblem(new Result<object>())).StatusCode);
    }
}
