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

public sealed class GlobalExceptionHandlerEnhancementTests
{
    private readonly GlobalExceptionHandler _handler = new(A.Fake<ILogger<GlobalExceptionHandler>>());

    [Theory]
    [InlineData(typeof(ValidationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(ManagementTaskLifecycleException), StatusCodes.Status409Conflict)]
    [InlineData(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    public async Task TryHandleAsync_ShouldReturnExpectedStatusAndTraceId(Type exceptionType, int expectedStatusCode)
    {
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        Exception exception = exceptionType.Name switch
        {
            nameof(ValidationException)
                => new ValidationException("Validation failed"),

            nameof(KeyNotFoundException)
                => new KeyNotFoundException("Task not found."),

            nameof(ManagementTaskLifecycleException)
                => new ManagementTaskLifecycleException("Conflict."),

            _ => new InvalidOperationException("Unexpected.")
        };

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.StatusCode.ShouldBe(expectedStatusCode);
        var response = await DeserializeAsync(context.Response.Body);
        response.TraceId.ShouldNotBeNullOrWhiteSpace();
    }

    private static async Task<ErrorResponseDto> DeserializeAsync(Stream stream)
    {
        stream.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<ErrorResponseDto>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return response.ShouldNotBeNull();
    }
}
