using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto registerDto)
        {
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, logout is typically handled on the client side by deleting the token.
            // Optionally, you can implement token blacklisting here.
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
