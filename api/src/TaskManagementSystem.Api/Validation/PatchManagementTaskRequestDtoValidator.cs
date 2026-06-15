using FluentValidation;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Validation;

public sealed class PatchManagementTaskRequestDtoValidator : AbstractValidator<PatchManagementTaskRequestDto>
{
    public PatchManagementTaskRequestDtoValidator()
    {
        RuleFor(request => request)
            .Must(request =>
                request.Title is not null ||
                request.Description is not null ||
                request.DueDate.HasValue ||
                request.UserId.HasValue)
            .WithMessage("At least one mutable field must be supplied.");

        When(request => request.Title is not null, () =>
        {
            RuleFor(request => request.Title!)
                .NotEmpty();
        });

        When(request => request.Description is not null, () =>
        {
            RuleFor(request => request.Description!)
                .NotEmpty();
        });

        When(request => request.DueDate.HasValue, () =>
        {
            RuleFor(request => request.DueDate!.Value)
                .NotEqual(default(DateTime))
                .WithMessage("DueDate must be a valid date.");
        });

        When(request => request.UserId.HasValue, () =>
        {
            RuleFor(request => request.UserId!.Value)
                .NotEmpty();
        });
    }
}
