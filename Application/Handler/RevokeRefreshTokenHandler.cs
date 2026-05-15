using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handlers;

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
{
    private readonly IAuthService _authService;

    public RevokeRefreshTokenHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
        return Unit.Value;
    }
}
