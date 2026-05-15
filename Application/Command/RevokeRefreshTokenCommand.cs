using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Command;

public record RevokeRefreshTokenCommand(string RefreshToken) : ICommand<Unit>;
