using api.DTOs;
using FluentValidation;

namespace api.Validators;

public class ReorderTodoDtoValidator : AbstractValidator<ReorderTodoDto>
{
    public ReorderTodoDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Todo Id is required");

        RuleFor(x => x.Position)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Position must be greater than or equal to 0");
    }
}