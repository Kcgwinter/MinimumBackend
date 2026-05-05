using System;
using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class PasswordForgotRequestDtoValidator : AbstractValidator<PasswordForgotRequestDto>
{
    public PasswordForgotRequestDtoValidator()
    {
        RuleFor(dto => dto.Email).NotEmpty().WithMessage("Email is required.");
    }
}
