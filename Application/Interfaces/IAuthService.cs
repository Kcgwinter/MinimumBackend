using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
        Task<string> LoginAsync(UserLoginDto loginDto);
        Task<bool> UserExistsAsync(string username);
    }
}