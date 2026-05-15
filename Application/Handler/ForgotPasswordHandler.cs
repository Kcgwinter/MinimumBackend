using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handler;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IAuthService _authService;

    public ForgotPasswordHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // The request.Dto holds the validated data (PasswordForgotRequestDto).
            // Call the service layer to perform the forgot password operation.
            await _authService.RequestPasswordResetAsync(request.Dto.Email);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            // Catch all other unexpected system errors, log them, and wrap them.
            throw new ApplicationException(
                $"An unexpected error occurred during forgot password: {ex.Message}",
                ex
            );
        }
    }
}
