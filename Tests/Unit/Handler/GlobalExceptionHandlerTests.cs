namespace Unit.Handler;

using System.Collections.Generic;

using Api.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public sealed class GlobalExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsService;
    private readonly Mock<IWebHostEnvironment> _environment;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _problemDetailsService = new Mock<IProblemDetailsService>();
        _environment = new Mock<IWebHostEnvironment>();

        _environment.Setup(e => e.EnvironmentName).Returns("Production");

        _handler = new GlobalExceptionHandler(
            _problemDetailsService.Object,
            _environment.Object,
            NullLogger<GlobalExceptionHandler>.Instance);
    }

    private static HttpContext BuildHttpContext(string path = "/api/test")
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Method = "GET";
        return context;
    }

    [Fact]
    public async Task TryHandleAsync_NotFoundException_Sets404StatusCode()
    {
        var context = BuildHttpContext();
        var exception = new NotFoundException("Order 42 not found.");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_NotFoundException_ReturnsTrueIndicatingHandled()
    {
        var context = BuildHttpContext();
        var exception = new NotFoundException("Order 42 not found.");

        bool handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
    }

    [Fact]
    public async Task TryHandleAsync_NotFoundException_WritesProblemDetailsWithCorrectShape()
    {
        var context = BuildHttpContext("/api/orders/42");
        var exception = new NotFoundException("Order 42 not found.");
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.NotNull(capturedDetails);
        Assert.Equal(StatusCodes.Status404NotFound, capturedDetails.Status);
        Assert.Equal("Not Found", capturedDetails.Title);
        Assert.Equal("Order 42 not found.", capturedDetails.Detail);
        Assert.Equal("/api/orders/42", capturedDetails.Instance);
    }

    [Fact]
    public async Task TryHandleAsync_ConflictException_Sets409StatusCode()
    {
        var context = BuildHttpContext();
        var exception = new ConflictException("Duplicate email address.");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ConflictException_WritesProblemDetailsWithCorrectTitle()
    {
        var context = BuildHttpContext();
        var exception = new ConflictException("Duplicate email address.");
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal("Conflict", capturedDetails!.Title);
        Assert.Equal("Duplicate email address.", capturedDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_Sets422StatusCode()
    {
        var context = BuildHttpContext();
        var exception = new ValidationException("Validation failed.", new Dictionary<string, string[]>
        {
            { "Email", ["Email is required.", "Email must be valid."] },
            { "Price", ["Price must be positive."] },
        });

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_WritesValidationProblemDetailsWithErrors()
    {
        var context = BuildHttpContext();
        var errors = new Dictionary<string, string[]>
        {
            { "Email", ["Email is required."] },
            { "Price", ["Price must be positive."] },
        };
        var exception = new ValidationException("Validation failed.", errors);
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        var validationDetails = Assert.IsType<ValidationProblemDetails>(capturedDetails);
        Assert.Equal(2, validationDetails.Errors.Count);
        Assert.Contains("Email", validationDetails.Errors.Keys);
        Assert.Contains("Price", validationDetails.Errors.Keys);
    }

    [Fact]
    public async Task TryHandleAsync_UnexpectedException_Sets500StatusCode()
    {
        var context = BuildHttpContext();
        var exception = new InvalidOperationException("Internal state corruption.");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_UnexpectedException_InProduction_RedactsDetail()
    {
        _environment.Setup(e => e.EnvironmentName).Returns("Production");
        var context = BuildHttpContext();
        var exception = new InvalidOperationException("Sensitive internal message.");
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.NotNull(capturedDetails);
        Assert.DoesNotContain("Sensitive internal message.", capturedDetails.Detail ?? string.Empty);
        Assert.Equal("An unexpected error occurred. Please try again later.", capturedDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_UnexpectedException_InDevelopment_ExposesDetail()
    {
        _environment.Setup(e => e.EnvironmentName).Returns("Development");
        var context = BuildHttpContext();
        var exception = new InvalidOperationException("Sensitive internal message.");
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal("Sensitive internal message.", capturedDetails!.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_AnyException_AlwaysReturnsTrue()
    {
        var context = BuildHttpContext();
        var exceptions = new Exception[]
        {
            new NotFoundException("not found"),
            new ConflictException("conflict"),
            new ValidationException("invalid"),
            new InvalidOperationException("unexpected"),
            new ArgumentException("arg"),
        };

        foreach (var exception in exceptions)
        {
            bool handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
            Assert.True(handled, $"Handler returned false for {exception.GetType().Name}");
        }
    }

    [Fact]
    public async Task TryHandleAsync_NotFoundException_InProduction_ExposesDetailUnredacted()
    {
        _environment.Setup(e => e.EnvironmentName).Returns("Production");
        var context = BuildHttpContext();
        var exception = new NotFoundException("Order 99 not found.");
        ProblemDetails? capturedDetails = null;

        _problemDetailsService
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(ctx => capturedDetails = ctx.ProblemDetails);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal("Order 99 not found.", capturedDetails!.Detail);
    }
}
