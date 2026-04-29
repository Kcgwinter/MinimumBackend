using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public sealed record Response(string AccessToken, string RefreshToken);

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto)
        {
            if (await UserExistsAsync(registerDto.Username))
                throw new ApplicationException("Username already exists");

            CreatePasswordHash(
                registerDto.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt
            );

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                EmailConfirmed = false,
                EmailConfirmationToken = GenerateEmailConfirmationToken(),
                EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24),
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        private string GenerateEmailConfirmationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<Response> LoginAsync(UserLoginDto loginDto)
        {
            var user = (
                await _unitOfWork.Users.FindAsync(u => u.Username == loginDto.Username)
            ).FirstOrDefault();

            if (
                user == null
                || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)
            )
                throw new ApplicationException("Invalid credentials");

            if (!user.EmailConfirmed)
                throw new ApplicationException("Email not confirmed");

            return CreateToken(user);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _unitOfWork.Users.ExistsAsync(u => u.Username == username);
        }

        private void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt
        )
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        //TODO
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        // public async Task<string> RefreshTokenAsync(string token)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var principal = tokenHandler.ValidateToken(
        //         token,
        //         new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true, // Ensure expired tokens are rejected
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = _configuration["Jwt:Issuer"],
        //             ValidAudience = _configuration["Jwt:Audience"],
        //             IssuerSigningKey = new SymmetricSecurityKey(
        //                 Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        //             ),
        //         },
        //         out SecurityToken validatedToken
        //     );

        //     if (
        //         validatedToken is not JwtSecurityToken jwtToken
        //         || !jwtToken.Header.Alg.Equals(
        //             SecurityAlgorithms.HmacSha512,
        //             StringComparison.InvariantCultureIgnoreCase
        //         )
        //     )
        //         throw new ApplicationException("Invalid token");

        //     var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     if (userId == null)
        //         throw new ApplicationException("Invalid token");

        //     // Check if the refresh token is valid and not revoked
        //     var refreshToken = await _unitOfWork.RefreshTokens.GetAsync(userId, token);
        //     if (refreshToken == null || refreshToken.IsRevoked)
        //         throw new ApplicationException("Invalid or revoked refresh token");

        //     // Revoke the current refresh token
        //     refreshToken.IsRevoked = true;
        //     await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken);
        //     await _unitOfWork.CompleteAsync();

        //     // Generate a new JWT token
        //     return CreateToken(refreshToken.User);
        // }

        public Task<bool> LogoutAsync(UserLogoutDto logoutDto)
        {
            // In a stateless JWT authentication system, logout is typically handled on the client side
            // by simply deleting the token.
            // For demonstration purposes, we'll just return true to indicate a successful logout.
            return Task.FromResult(true);
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var user = _unitOfWork.Users.FindAsync(u => u.Email == email).Result.FirstOrDefault();
            if (user == null)
                throw new ApplicationException("Email not found");

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
            _ = _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            //Todo: Send email with token
            Debug.WriteLine($"Password reset token for {email}: {token}");
        }

        public Task ResetPasswordAsync(PasswordResetRequestDto dto)
        {
            var user = _unitOfWork
                .Users.FindAsync(u => u.PasswordResetToken == dto.Token)
                .Result.FirstOrDefault();

            if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
                throw new ApplicationException("Invalid or expired token");

            CreatePasswordHash(dto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            _ = _unitOfWork.Users.UpdateAsync(user);
            return _unitOfWork.CompleteAsync();
        }

        public Task ConfirmEmailAsync(string token)
        {
            var user = _unitOfWork
                .Users.FindAsync(u => u.EmailConfirmationToken == token)
                .Result.FirstOrDefault();

            if (user == null || user.EmailConfirmationTokenExpires < DateTime.UtcNow)
                throw new ApplicationException("Invalid or expired token");

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;

            _ = _unitOfWork.Users.UpdateAsync(user);
            return _unitOfWork.CompleteAsync();
        }

        public Task RequestEmailConfirmationAsync(string email)
        {
            var user = _unitOfWork.Users.FindAsync(u => u.Email == email).Result.FirstOrDefault();
            if (user == null)
                throw new ApplicationException("Email not found");

            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            _ = _unitOfWork.Users.UpdateAsync(user);
            return _unitOfWork.CompleteAsync();
        }
    }
}
