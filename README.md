# AlfaHackaton.Controllers

`BaseController.OkOrProblem` returns successful content unchanged. For errors,
`Error.HttpStatusCode` takes precedence when provided; otherwise the existing
`ErrorType` mapping applies (400, 401, 403, 404, or 500). Error responses retain
`{ ErrorMessage = Error.Title }`, except the existing HTTP 400 response with
non-null result content, which returns that content.

Run the contract tests with:

```sh
dotnet test Tests/AlfaHackaton.Controllers.Tests.csproj -c Release
```
