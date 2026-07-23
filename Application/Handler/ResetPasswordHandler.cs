using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using Core.Exceptions;
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
            await _authService.ResetPasswordAsync(request.Dto);
            return Unit.Value;
        }
        catch (Exception ex) when (ex is InvalidTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"An unexpected error occurred during reset password: {ex.Message}",
                ex
            );
        }
    }
}
