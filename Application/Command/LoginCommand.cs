using Application.Interfaces;
using Core.DTOs;

namespace Application.Command;

public record LoginCommand(UserLoginDto LoginDto) : ICommand<TokenResponseDto>;
