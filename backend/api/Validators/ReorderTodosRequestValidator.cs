using api.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;

namespace api.Validators;

public class ReorderTodosRequestValidator : AbstractValidator<ReorderTodosRequest>
{
    public ReorderTodosRequestValidator()
    {
        RuleFor(x => x.Todos)
            .NotNull()
            .WithMessage("Todos is required")
            .Must(t =>
            {
                var list = t as IList<ReorderTodoDto> ?? t.ToList();
                return list.Count > 0;
            }).WithMessage("Todos is empty");

        RuleForEach(x => x.Todos)
            .SetValidator(new ReorderTodoDtoValidator());

        RuleFor(x => x.Todos)
            .Must(t =>
            {
                var list = t as IList<ReorderTodoDto> ?? t.ToList();
                return list.Select(i => i.Id).Distinct().Count() == list.Count;
            })
            .WithMessage("Todos must not contain duplicate ids");

        RuleFor(x => x.Todos)
            .Must(t =>
            {
                var list = t as IList<ReorderTodoDto> ?? t.ToList();
                return list.Select(i => i.Position).Distinct().Count() == list.Count;
            })
            .WithMessage("Todos must not contain duplicate positions");
    }
}