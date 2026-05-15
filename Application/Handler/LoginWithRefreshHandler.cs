using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handlers;

public class LoginWithRefreshCommandHandler
    : IRequestHandler<LoginWithRefreshCommand, TokenResponseDto>
{
    private readonly IAuthService _authService;

    public LoginWithRefreshCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponseDto> Handle(
        LoginWithRefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var newToken = await _authService.LoginWithRefreshAsync(request.UserToken);
        return new TokenResponseDto(newToken.accessToken, request.UserToken);
    }
}
