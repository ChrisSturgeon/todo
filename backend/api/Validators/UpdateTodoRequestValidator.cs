using api.DTOs;
using FluentValidation;

namespace api.Validators;

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .When(n => !string.IsNullOrWhiteSpace(n.Name))
            .WithMessage("Todo name is required")
            .Length(3, 50)
            .When(n => !string.IsNullOrWhiteSpace(n.Name))
            .WithMessage("Todo name must be between 3 and 50 characters");

        RuleFor(u => u.Description)
            .Length(3, 100)
            .When(d => !string.IsNullOrWhiteSpace(d.Description))
            .WithMessage("Todo description must be between 3 and 100 characters");
    }
}