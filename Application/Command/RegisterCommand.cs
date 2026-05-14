using System;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Command;

public record RegisterCommand(UserRegisterDto RegisterDto) : ICommand<UserResponseDto>;
