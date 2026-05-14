using System.Diagnostics;
using Application.Command;
using Application.Interfaces;
using Core.DTOs;
using Core.Validators;
using FluentValidation;
using MediatR;
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
        private readonly IMediator _mediator;

        public AuthController(
            IAuthService authService,
            IValidator<PasswordForgotRequestDto> passwordForgotValidator,
            IMediator mediator
        )
        {
            _authService = authService;
            _passwordForgotValidator = passwordForgotValidator;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto registerDto)
        {
            var userDto = await _mediator.Send(new RegisterCommand(registerDto));

            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            try
            {
                var token = await _mediator.Send(new LoginCommand(loginDto));
                return Ok(new { token });
            }
            // ExceptionMiddleware will handle unexpected errors, but we catch ApplicationException for specific UI feedback
            catch (Exception ex) when (ex is ApplicationException or UnauthorizedAccessException)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("loginRefresh")]
        public async Task<ActionResult> LoginWithRefresh(string RefreshToken)
        {
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
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(PasswordForgotRequestDto dto)
        {
            var validationResult = await _passwordForgotValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
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
