using Core.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
        Task<(string accessToken, string refreshToken)> LoginAsync(UserLoginDto loginDto);
        Task<(string accessToken, string refreshToken)> LoginWithRefreshAsync(string refreshToken);

        Task<bool> UserExistsAsync(string username);
        Task<bool> LogoutAsync(UserLogoutDto logoutDto);
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(PasswordResetRequestDto dto);
        Task ConfirmEmailAsync(string token);

        Task RequestEmailConfirmationAsync(string email);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
