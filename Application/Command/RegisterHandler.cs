using System;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand, UserResponseDto>
{
    private readonly IAuthService _authService;
    public RegisterHandler(IAuthService authService) => _authService = authService;

    public async Task<UserResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    => await _authService.RegisterAsync(request.RegisterDto);
}
