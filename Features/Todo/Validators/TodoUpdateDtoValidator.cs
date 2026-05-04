using Features.Todo.DTOs;
using FluentValidation;

namespace Features.Todo.Validators
{
    public class TodoUpdateDtoValidator : AbstractValidator<TodoUpdateDto>
    {
        public TodoUpdateDtoValidator()
        {
            RuleFor(dto => dto.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.").WithMessage("Title must be provided if changed.");

            RuleFor(dto => dto.Description)
                .NotEmpty().WithMessage("Description cannot be empty.");

            RuleFor(dto => dto.IsCompleted)
                .NotNull().WithMessage("Completion status must be provided.");

            RuleFor(dto => dto.DueDate)
                .NotNull().WithMessage("Due Date must be provided.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Due Date cannot be in the past.");
        }
    }
}