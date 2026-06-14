namespace TaskManagementSystem.Api.Models;

public sealed class ErrorResponseDto
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public IReadOnlyCollection<ValidationErrorDto> Errors { get; set; } = Array.Empty<ValidationErrorDto>();

    public string TraceId { get; set; } = string.Empty;
}
