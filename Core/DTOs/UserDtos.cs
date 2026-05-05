using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class UserLogoutDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PasswordForgotRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class PasswordResetRequestDto
    {
        public string Token { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
