using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handler;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly IAuthService _authService;

    public ConfirmEmailHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // The request.Token holds the validated data (ConfirmEmailCommand).
            // Call the service layer to perform the confirm email operation.
            await _authService.ConfirmEmailAsync(request.Token);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            // Catch all other unexpected system errors, log them, and wrap them.
            throw new ApplicationException(
                $"An unexpected error occurred during confirm email: {ex.Message}",
                ex
            );
        }
    }
}
