using FakeItEasy;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shouldly;
using TaskManagementSystem.Api.ExceptionHandling;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Api.UnitTests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    private readonly ILogger<GlobalExceptionHandler> _logger = A.Fake<ILogger<GlobalExceptionHandler>>();
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(_logger);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnBadRequest_ForValidationException()
    {
        var context = CreateHttpContext();
        var exception = new ValidationException("Validation failed", new[]
        {
            new FluentValidation.Results.ValidationFailure("Title", "Title is required.")
        });

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        response.Message.ShouldBe("One or more validation errors occurred.");
        response.Errors.ShouldContain(error => error.PropertyName == "Title");
        AssertLogWritten(LogLevel.Warning, "validation", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnNotFound_ForKeyNotFoundException()
    {
        var context = CreateHttpContext();
        var exception = new KeyNotFoundException("Task not found.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        response.Message.ShouldBe("Task not found.");
        response.TraceId.ShouldNotBeNullOrWhiteSpace();
        AssertLogWritten(LogLevel.Warning, "Task not found.", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnBadRequest_ForDomainValidationException()
    {
        var context = CreateHttpContext();
        var exception = new InvalidManagementTaskException("Title cannot be empty.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        response.Message.ShouldBe("Title cannot be empty.");
        AssertLogWritten(LogLevel.Warning, "Title cannot be empty.", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnConflict_ForManagementTaskLifecycleException()
    {
        var context = CreateHttpContext();
        var exception = new ManagementTaskLifecycleException("Transition is not allowed.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
        response.Message.ShouldBe("Transition is not allowed.");
        response.TraceId.ShouldNotBeNullOrWhiteSpace();
        AssertLogWritten(LogLevel.Warning, "Transition is not allowed.", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnConflict_ForDuplicateManagementUserException()
    {
        var context = CreateHttpContext();
        var exception = new DuplicateManagementUserException("A user already exists.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
        response.Message.ShouldBe("A user already exists.");
        AssertLogWritten(LogLevel.Warning, "A user already exists.", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnUnauthorized_ForInvalidManagementUserCredentialsException()
    {
        var context = CreateHttpContext();
        var exception = new InvalidManagementUserCredentialsException("The supplied credentials are invalid.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        response.Message.ShouldBe("The supplied credentials are invalid.");
        AssertLogWritten(LogLevel.Warning, "The supplied credentials are invalid.", context.TraceIdentifier);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnInternalServerError_ForUnexpectedException()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(context, new InvalidOperationException("Boom"), CancellationToken.None);
        var response = await ReadResponseAsync(context);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        response.Message.ShouldBe("An unexpected error occurred.");
        AssertLogWritten(LogLevel.Error, "Unhandled exception", context.TraceIdentifier);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static async Task<ErrorResponseDto> ReadResponseAsync(HttpContext httpContext)
    {
        httpContext.Response.Body.Position = 0;

        var response = await JsonSerializer.DeserializeAsync<ErrorResponseDto>(
            httpContext.Response.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return response.ShouldNotBeNull();
    }

    private void AssertLogWritten(LogLevel level, string expectedText, string traceId)
    {
        A.CallTo(_logger).Where(call =>
            call.Method.Name == nameof(ILogger.Log) &&
            call.GetArgument<LogLevel>(0) == level &&
            call.GetArgument<object>(2).ToString()!.Contains(expectedText, StringComparison.OrdinalIgnoreCase) &&
            call.GetArgument<object>(2).ToString()!.Contains(traceId, StringComparison.Ordinal))
            .MustHaveHappened();
    }
}
