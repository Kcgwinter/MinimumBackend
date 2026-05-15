using Application.Interfaces;
using Core.DTOs;

namespace Application.Command;

public record LoginWithRefreshCommand(string UserToken) : ICommand<TokenResponseDto>;
