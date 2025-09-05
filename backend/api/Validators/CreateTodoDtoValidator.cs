using api.DTOs;
using FluentValidation;

namespace api.Validators;

public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Todo name is required");
        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("Todo name must be between 3 and 100 characters");
        RuleFor(x => x.Description)
            .Length(3, 100)
            .WithMessage("Todo description must be between 3 and 100 characters");
    }
}