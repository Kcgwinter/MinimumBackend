using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        // The business logic is still encapsulated in the service for now,
        // but the entry point is now this handler.
        var (accessToken, refreshToken) = await _authService.LoginAsync(request.LoginDto);
        return new TokenResponseDto(accessToken, refreshToken);
    }
}
