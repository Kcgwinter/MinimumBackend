using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

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
            // The request.RegisterDto holds the validated data (UserRegisterDto).
            // Call the service layer to perform the registration.
            var user = await _authService.RegisterAsync(request.RegisterDto);
            return user;
        }
        catch (Exception ex)
        {
            // Catch all other unexpected system errors, log them, and wrap them.
            throw new ApplicationException(
                $"An unexpected error occurred during registration: {ex.Message}",
                ex
            );
        }
    }
}
