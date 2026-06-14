using FluentValidation;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Validation;

public sealed class CreateManagementUserRequestDtoValidator : AbstractValidator<CreateManagementUserRequestDto>
{
    public CreateManagementUserRequestDtoValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty();

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
