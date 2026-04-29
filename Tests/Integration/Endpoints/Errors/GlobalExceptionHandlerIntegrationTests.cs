namespace Integration.Endpoints.Errors;

using System.Net;
using System.Net.Http.Json;

using Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Integration tests verifying that <see cref="Api.Exceptions.GlobalExceptionHandler"/>
/// returns RFC 9457-compliant responses over real HTTP for each domain exception type.
/// </summary>
public sealed class GlobalExceptionHandlerIntegrationTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GET_TestNotFound_WhenNotFoundExceptionIsThrown_Returns404()
    {
        var response = await Client.GetAsync("/test/not-found");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_TestNotFound_WhenNotFoundExceptionIsThrown_ReturnsProblemJsonContentType()
    {
        var response = await Client.GetAsync("/test/not-found");

        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GET_TestNotFound_WhenNotFoundExceptionIsThrown_ReturnsProblemDetailsWithCorrectShape()
    {
        var response = await Client.GetAsync("/test/not-found");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
        Assert.Equal("Not Found", problem.Title);
        Assert.Equal("Test resource not found.", problem.Detail);
        Assert.Equal("/test/not-found", problem.Instance);
    }

    [Fact]
    public async Task GET_TestConflict_WhenConflictExceptionIsThrown_Returns409()
    {
        var response = await Client.GetAsync("/test/conflict");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GET_TestConflict_WhenConflictExceptionIsThrown_ReturnsProblemDetailsWithCorrectShape()
    {
        var response = await Client.GetAsync("/test/conflict");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(409, problem.Status);
        Assert.Equal("Conflict", problem.Title);
        Assert.Equal("Duplicate test resource.", problem.Detail);
    }

    [Fact]
    public async Task GET_TestValidation_WhenValidationExceptionIsThrown_Returns422()
    {
        var response = await Client.GetAsync("/test/validation");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GET_TestValidation_WhenValidationExceptionIsThrown_ReturnsProblemJsonContentType()
    {
        var response = await Client.GetAsync("/test/validation");

        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GET_TestValidation_WhenValidationExceptionIsThrown_ReturnsValidationProblemDetailsWithErrors()
    {
        var response = await Client.GetAsync("/test/validation");
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(422, problem.Status);
        Assert.Equal("Unprocessable Entity", problem.Title);
        Assert.Contains("Field", problem.Errors.Keys);
    }

    [Fact]
    public async Task GET_TestError_WhenUnhandledExceptionIsThrown_Returns500()
    {
        var response = await Client.GetAsync("/test/error");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GET_TestError_WhenUnhandledExceptionIsThrown_InDevelopment_ExposesExceptionDetail()
    {
        var response = await Client.GetAsync("/test/error");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal("Unhandled test error.", problem.Detail);
    }
}
