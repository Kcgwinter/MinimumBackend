using System;
using System.Linq;
using System.Security.Cryptography;
using Application.Interfaces;
using Application.Services;
using Core.DTOs;
using Core.Entities;
using FluentAssertions.Extensions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Test.Services
{
    public class AuthServiceTests
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;

        public AuthServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Jwt:Key"] = "ThisIsASuperSecretKeyForJWT1234567890",
                        ["Jwt:Issuer"] = "https://localhost:5001",
                        ["Jwt:Audience"] = "https://localhost:5001",
                    }
                )
                .Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            _context = new AppDbContext(options);

            // Configure entity types
            _context.Database.EnsureCreated();

            var mapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserResponseDto>();
            }).CreateMapper();

            _authService = new AuthService(mapper, configuration, _context);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldRegisterUser()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
            Assert.True(result.CreatedAt.CompareTo(DateTime.UtcNow) <= 0);
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateUsername_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };

            // Act
            await _authService.RegisterAsync(registerDto);

            var duplicateDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "different@example.com",
                Password = "DifferentPassword123",
            };

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(() =>
                _authService.RegisterAsync(duplicateDto)
            );
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "invalid-email",
                Password = "SecurePassword123",
            };

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.RegisterAsync(registerDto)
            );
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "SecurePassword123",
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result.accessToken);
            Assert.NotNull(result.refreshToken);
            Assert.True(DateTime.UtcNow.AddHours(2).CompareTo(result.accessToken) > 0);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ShouldThrowException()
        {
            // Arrange
            var loginDto = new UserLoginDto { Username = "nonexistent", Password = "password" };

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.LoginAsync(loginDto)
            );
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto { Username = "testuser", Password = "WrongPassword" };

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.LoginAsync(loginDto)
            );
        }

        [Fact]
        public async Task LoginWithRefreshAsync_WithValidToken_ShouldReturnNewTokens()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "SecurePassword123",
            };
            var loginResult = await _authService.LoginAsync(loginDto);
            var refreshToken = loginResult.refreshToken;

            // Act
            var result = await _authService.LoginWithRefreshAsync(refreshToken);

            // Assert
            Assert.NotNull(result.accessToken);
            Assert.NotNull(result.refreshToken);
            Assert.NotEqual(refreshToken, result.refreshToken); // New token should be different
        }

        [Fact]
        public async Task LoginWithRefreshAsync_WithExpiredToken_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "SecurePassword123",
            };
            var loginResult = await _authService.LoginAsync(loginDto);
            var refreshToken = loginResult.refreshToken;

            // Arrange - Manually expire the token
            var refresh = _context.RefreshTokens.Find(refreshToken);
            if (refresh != null)
            {
                refresh.Expires = DateTime.UtcNow.AddMinutes(-1);
                _context.RefreshTokens.Update(refresh);
                await _context.SaveChangesAsync();
            }

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.LoginWithRefreshAsync(refreshToken)
            );
        }

        [Fact]
        public async Task RequestPasswordResetAsync_WithValidEmail_ShouldCreateResetToken()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            // Act
            await _authService.RequestPasswordResetAsync("test@example.com");

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.NotNull(user.PasswordResetToken);
            Assert.NotNull(user.PasswordResetTokenExpires);
            Assert.True(user.PasswordResetTokenExpires.Value > DateTime.UtcNow);
        }

        [Fact]
        public async Task RequestPasswordResetAsync_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.RequestPasswordResetAsync("invalid@example.com")
            );
        }

        [Fact]
        public async Task ResetPasswordAsync_WithValidToken_ShouldResetPassword()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "OldPassword123",
            };
            await _authService.RegisterAsync(registerDto);

            await _authService.RequestPasswordResetAsync("test@example.com");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            var resetDto = new PasswordResetRequestDto
            {
                Token = user.PasswordResetToken,
                NewPassword = "NewPassword456",
            };

            // Act
            await _authService.ResetPasswordAsync(resetDto);

            // Assert
            Assert.NotNull(user);
            Assert.Null(user.PasswordResetToken);
            Assert.Null(user.PasswordResetTokenExpires);
        }

        [Fact]
        public async Task ResetPasswordAsync_WithExpiredToken_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "OldPassword123",
            };
            await _authService.RegisterAsync(registerDto);

            await _authService.RequestPasswordResetAsync("test@example.com");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            // Expire the token
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(-1);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var resetDto = new PasswordResetRequestDto
            {
                Token = user.PasswordResetToken,
                NewPassword = "NewPassword456",
            };

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.ResetPasswordAsync(resetDto)
            );
        }

        [Fact]
        public async Task RequestEmailConfirmationAsync_WithValidEmail_ShouldCreateConfirmationToken()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            // Act
            await _authService.RequestEmailConfirmationAsync("test@example.com");

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.False(user.EmailConfirmed);
            Assert.NotNull(user.EmailConfirmationToken);
            Assert.NotNull(user.EmailConfirmationTokenExpires);
            Assert.True(user.EmailConfirmationTokenExpires.Value > DateTime.UtcNow);
        }

        [Fact]
        public async Task ConfirmEmailAsync_WithValidToken_ShouldConfirmEmail()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            await _authService.RequestEmailConfirmationAsync("test@example.com");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            var confirmToken = user.EmailConfirmationToken;

            // Act
            await _authService.ConfirmEmailAsync(confirmToken!);

            // Assert
            Assert.NotNull(user);
            Assert.True(user.EmailConfirmed);
            Assert.Null(user.EmailConfirmationToken);
            Assert.Null(user.EmailConfirmationTokenExpires);
        }

        [Fact]
        public async Task ConfirmEmailAsync_WithExpiredToken_ShouldThrowException()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            await _authService.RequestEmailConfirmationAsync("test@example.com");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            // Expire the token
            user?.EmailConfirmationTokenExpires = DateTime.UtcNow.AddMinutes(-1);
            _context.Users.Update(user!);
            await _context.SaveChangesAsync();

            var confirmToken = user!.EmailConfirmationToken;

            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.ConfirmEmailAsync(confirmToken!)
            );
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_WithValidToken_ShouldRevokeAndReturnTrue()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "SecurePassword123",
            };
            var loginResult = await _authService.LoginAsync(loginDto);
            var refreshToken = loginResult.refreshToken;

            // Act
            await _authService.RevokeRefreshTokenAsync(refreshToken);

            // Assert
            var revokedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken
            );
            Assert.NotNull(revokedToken);
            Assert.True(revokedToken.Expires < DateTime.UtcNow);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_WithInvalidToken_ShouldThrowException()
        {
            // Assert
            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await _authService.RevokeRefreshTokenAsync("invalid-token")
            );
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnTrue()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "SecurePassword123",
            };
            await _authService.LoginAsync(loginDto);

            // Act
            var result = await _authService.LogoutAsync(
                new UserLogoutDto { Id = 1, Username = "testuser" }
            );

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithExistingUser_ShouldReturnTrue()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            // Act
            var exists = await _authService.UserExistsAsync("testuser");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UserExistsAsync_WithNonExistingUser_ShouldReturnFalse()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123",
            };
            await _authService.RegisterAsync(registerDto);

            // Act
            var exists = await _authService.UserExistsAsync("nonexistent");

            // Assert
            Assert.False(exists);
        }
    }
}
