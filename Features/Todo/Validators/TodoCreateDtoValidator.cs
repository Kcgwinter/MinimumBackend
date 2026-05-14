using Features.Todo.DTOs;
using FluentValidation;

namespace Features.Todo.Validators
{
    public class TodoCreateDtoValidator : AbstractValidator<TodoCreateDto>
    {
        public TodoCreateDtoValidator()
        {
            RuleFor(dto => dto.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .Length(3, 100)
                .WithMessage("Title must be between 3 and 100 characters.");

            RuleFor(dto => dto.Description).NotEmpty().WithMessage("Description cannot be empty.");

            RuleFor(dto => dto.DueDate)
                .NotNull()
                .WithMessage("Due Date must be specified.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Due Date cannot be in the past.");
        }
    }
}
