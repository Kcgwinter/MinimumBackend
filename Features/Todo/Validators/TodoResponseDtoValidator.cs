using Features.Todo.DTOs;
using FluentValidation;

namespace Features.Todo.Validators
{
    public class TodoResponseDtoValidator : AbstractValidator<TodoResponseDto>
    {
        public TodoResponseDtoValidator()
        {
            // Basic checks for data integrity, assuming these values should always be present
            RuleFor(dto => dto.Title).NotEmpty();
            RuleFor(dto => dto.Description).NotEmpty();
            RuleFor(dto => dto.DueDate).NotNull();
            RuleFor(dto => dto.CreatedAt).NotNull();
        }
    }
}