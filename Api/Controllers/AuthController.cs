using System.Diagnostics;
using Application.Interfaces;
using Core.DTOs;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<UserRegisterDto> _userRegisterValidator;
        private readonly IValidator<UserLoginDto> _userLoginValidator;
        private readonly IValidator<PasswordForgotRequestDto> _passwordForgotValidator;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
           IValidator<UserRegisterDto> userRegisterValidator;
            IValidator<UserLoginDto> userLoginValidator;
            IValidator<PasswordForgotRequestDto> _passwordForgotValidator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto registerDto)
        {
            var validationResult = await _userRegisterValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); // Assumes you have an extension method to format errors
            }

            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            Console.WriteLine("Login attempt received.");

            var validationResult = await _userLoginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); // Assumes you have an extension method to format errors
            }

            try
            {
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { token });
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("loginRefresh")]
        public async Task<ActionResult> LoginWithRefresh(string RefreshToken)
        {
            Debug.WriteLine("Login with refresh attempt received.");
            try
            {
                var newToken = await _authService.LoginWithRefreshAsync(RefreshToken);
                return Ok(new { token = newToken });
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("revoke-refresh")]
        public async Task<ActionResult> RevokeRefreshToken(string RefreshToken)
        {
            Debug.WriteLine("Revoke refresh token attempt received.");
            try
            {
                await _authService.RevokeRefreshTokenAsync(RefreshToken);
                return Ok(new { message = "Refresh token revoked successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, logout is typically handled on the client side by deleting the token.
            // Optionally, you can implement token blacklisting here.
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(PasswordForgotRequestDto dto)
        {
            var validationResult = await _passwordForgotValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); // Assumes you have an extension method to format errors
            }
            await _authService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { message = "If that email exists, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordResetRequestDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password has been reset successfully." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            await _authService.ConfirmEmailAsync(token);
            return Ok(new { message = "Email confirmed successfully." });
        }
    }
}
