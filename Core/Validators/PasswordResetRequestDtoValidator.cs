using System;
using System.Data;
using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class PasswordResetRequestDtoValidator : AbstractValidator<PasswordResetRequestDto>
{
    public PasswordResetRequestDtoValidator()
    {
        RuleFor(dto => dto.Token).NotEmpty().WithMessage("Token is required.");
    }
}
