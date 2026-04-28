using Core.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
        Task<string> LoginAsync(UserLoginDto loginDto);
        Task<bool> UserExistsAsync(string username);
        Task<bool> LogoutAsync(UserLogoutDto logoutDto);
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(PasswordResetRequestDto dto);
        Task ConfirmEmailAsync(string token);

        Task RequestEmailConfirmationAsync(string email);
    }
}
