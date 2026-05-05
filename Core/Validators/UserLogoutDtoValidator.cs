using System;
using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class UserLogoutDtoValidator : AbstractValidator<UserLogoutDto>
{
    public UserLogoutDtoValidator()
    {
        RuleFor(dto => dto.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 100)
            .WithMessage("Username must be between 3 and 100 characters.");
    }
}
