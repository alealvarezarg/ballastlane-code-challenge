namespace TaskManagementSystem.Api.Models;

public sealed class ValidationErrorDto
{
    public ValidationErrorDto()
    {
    }

    public ValidationErrorDto(string propertyName, string message)
    {
        PropertyName = propertyName;
        Message = message;
    }

    public string PropertyName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
