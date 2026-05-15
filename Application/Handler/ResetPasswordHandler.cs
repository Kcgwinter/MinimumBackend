using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handler;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IAuthService _authService;

    public ResetPasswordHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // The request.Dto holds the validated data (PasswordResetRequestDto).
            // Call the service layer to perform the reset password operation.
            await _authService.ResetPasswordAsync(request.Dto);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            // Catch all other unexpected system errors, log them, and wrap them.
            throw new ApplicationException(
                $"An unexpected error occurred during reset password: {ex.Message}",
                ex
            );
        }
    }
}
