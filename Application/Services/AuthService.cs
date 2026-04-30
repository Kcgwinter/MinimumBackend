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
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public sealed record Response(string AccessToken, string RefreshToken);

        public AuthService(IMapper mapper, IConfiguration configuration, AppDbContext context)
        {
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
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

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        private string GenerateEmailConfirmationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(
            UserLoginDto loginDto
        )
        {
            var user = (
                await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username)
            );

            if (
                user == null
                || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)
            )
                throw new ApplicationException("Invalid credentials");

            if (!user.EmailConfirmed)
                throw new ApplicationException("Email not confirmed");

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                User = user,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return (CreateToken(user), refreshToken.Token);
        }

        public async Task<(string accessToken, string refreshToken)> LoginWithRefreshAsync(
            string refreshToken
        )
        {
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken
            );
            if (existingToken == null || existingToken.Expires < DateTime.UtcNow)
                throw new ApplicationException("Invalid or expired refresh token");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingToken.UserId);
            if (user == null)
                throw new ApplicationException("User not found");

            // Revoke the old refresh token
            existingToken.Expires = DateTime.UtcNow; // Mark as expired
            _context.RefreshTokens.Update(existingToken);
            await _context.SaveChangesAsync();

            // Generate a new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                User = user,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return (CreateToken(user), newRefreshToken.Token);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
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

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = _context
                .RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken)
                .Result;
            if (token != null)
            {
                var userTokens = _context.Users.FirstOrDefaultAsync(u => u.Id == token.UserId);
                _context.RefreshTokens.Remove(token);

                token.Expires = DateTime.UtcNow; // Mark as expired
                _ = _context.RefreshTokens.Update(token);
                return _context.SaveChangesAsync();
            }

            throw new ApplicationException("Refresh token not found");
        }

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new ApplicationException("Email not found");

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
            _ = _context.Users.Update(user);
            await _context.SaveChangesAsync();

            //Todo: Send email with token
            Debug.WriteLine($"Password reset token for {email}: {token}");
        }

        public Task ResetPasswordAsync(PasswordResetRequestDto dto)
        {
            var user = _context
                .Users.FirstOrDefaultAsync(u => u.PasswordResetToken == dto.Token)
                .Result;

            if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
                throw new ApplicationException("Invalid or expired token");

            CreatePasswordHash(dto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            _ = _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }

        public Task ConfirmEmailAsync(string token)
        {
            var user = _context
                .Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token)
                .Result;

            if (user == null || user.EmailConfirmationTokenExpires < DateTime.UtcNow)
                throw new ApplicationException("Invalid or expired token");

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;

            _ = _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }

        public Task RequestEmailConfirmationAsync(string email)
        {
            var user = _context.Users.FirstOrDefaultAsync(u => u.Email == email).Result;
            if (user == null)
                throw new ApplicationException("Email not found");

            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            _ = _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }
    }
}
