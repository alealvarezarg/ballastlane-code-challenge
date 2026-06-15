using FluentValidation;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Validation;

public sealed class LoginManagementUserRequestDtoValidator : AbstractValidator<LoginManagementUserRequestDto>
{
    public LoginManagementUserRequestDtoValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
