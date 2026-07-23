using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using Core.Exceptions;
using MediatR;

namespace Application.Handler;

public class RegisterHandler : IRequestHandler<RegisterCommand, UserResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UserResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var user = await _authService.RegisterAsync(request.RegisterDto);
            return user;
        }
        catch (Exception ex) when (ex is DuplicateUsernameException or InvalidCredentialsException or EmailNotConfirmedException or InvalidTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"An unexpected error occurred during registration: {ex.Message}",
                ex
            );
        }
    }
}
