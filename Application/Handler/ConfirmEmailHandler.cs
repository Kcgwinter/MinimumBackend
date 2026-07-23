using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using Core.Exceptions;
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
            await _authService.ConfirmEmailAsync(request.Token);
            return Unit.Value;
        }
        catch (Exception ex) when (ex is InvalidTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"An unexpected error occurred during confirm email: {ex.Message}",
                ex
            );
        }
    }
}
