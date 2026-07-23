using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using Core.Exceptions;
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
            await _authService.RequestPasswordResetAsync(request.Dto.Email);
            return Unit.Value;
        }
        catch (Exception ex) when (ex is EmailNotFoundException or InvalidTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"An unexpected error occurred during forgot password: {ex.Message}",
                ex
            );
        }
    }
}
