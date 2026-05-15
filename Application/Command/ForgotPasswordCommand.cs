using System;
using Application.Interfaces;
using Core.DTOs;
using MediatR;

namespace Application.Command;

public record ForgotPasswordCommand(PasswordForgotRequestDto Dto) : ICommand<Unit>;
