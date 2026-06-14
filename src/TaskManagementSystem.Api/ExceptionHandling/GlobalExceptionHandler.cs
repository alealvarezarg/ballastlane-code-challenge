using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = BuildErrorResponse(httpContext, exception, out var statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private static ErrorResponseDto BuildErrorResponse(
        HttpContext httpContext,
        Exception exception,
        out int statusCode)
    {
        var response = new ErrorResponseDto
        {
            TraceId = httpContext.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = statusCode;
                response.Message = "One or more validation errors occurred.";
                response.Errors = validationException.Errors
                    .Select(error => new ValidationErrorDto(error.PropertyName, error.ErrorMessage))
                    .ToArray();
                return response;

            case DomainValidationException domainValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = statusCode;
                response.Message = domainValidationException.Message;
                response.Errors = new[]
                {
                    new ValidationErrorDto(string.Empty, domainValidationException.Message)
                };
                return response;

            case KeyNotFoundException keyNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                response.StatusCode = statusCode;
                response.Message = keyNotFoundException.Message;
                return response;

            case ManagementTaskLifecycleException conflictException:
                statusCode = StatusCodes.Status409Conflict;
                response.StatusCode = statusCode;
                response.Message = conflictException.Message;
                return response;

            case DuplicateManagementUserException duplicateManagementUserException:
                statusCode = StatusCodes.Status409Conflict;
                response.StatusCode = statusCode;
                response.Message = duplicateManagementUserException.Message;
                return response;

            case InvalidManagementUserCredentialsException invalidCredentialsException:
                statusCode = StatusCodes.Status401Unauthorized;
                response.StatusCode = statusCode;
                response.Message = invalidCredentialsException.Message;
                return response;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                response.StatusCode = statusCode;
                response.Message = "An unexpected error occurred.";
                return response;
        }
    }
}
