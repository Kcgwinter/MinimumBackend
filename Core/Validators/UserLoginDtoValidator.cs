using System;
using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(dto => dto.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 100)
            .WithMessage("Username must be between 3 and 100 characters.");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.")
            .Length(6, 100);
    }
}
