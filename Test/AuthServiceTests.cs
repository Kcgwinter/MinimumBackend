using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Xunit.Abstractions;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;
    private readonly ITestOutputHelper _output;

    public AuthServiceTests(ITestOutputHelper output)
    {
        _output = output;
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestAuthService")
            .Options;

        _context = new AppDbContext(options);
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        _authService = new AuthService(_mapper, _configuration, _context);
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_CreatesUser()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
        };

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.NotNull(user.PasswordSalt);
        Assert.NotNull(user.EmailConfirmationToken);
        Assert.NotNull(user.EmailConfirmationTokenExpires);
    }

    [Fact]
    public async Task RegisterAsync_UserExists_ThrowsException()
    {
        // Arrange
        var user = new User { Username = "testuser" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
        };

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _authService.RegisterAsync(registerDto)
        );
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var loginDto = new UserLoginDto { Username = "testuser", Password = "Password123!" };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result.accessToken);
        Assert.NotNull(result.refreshToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var loginDto = new UserLoginDto { Username = "testuser", Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginWithRefreshAsync_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var refreshToken = new RefreshToken
        {
            Token = "refreshtoken123",
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LoginWithRefreshAsync("refreshtoken123");

        // Assert
        Assert.NotNull(result.accessToken);
        Assert.NotNull(result.refreshToken);
    }

    [Fact]
    public async Task LoginWithRefreshAsync_InvalidToken_ThrowsException()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _authService.LoginWithRefreshAsync("invalidtoken")
        );
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ValidToken_MarksAsExpired()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var refreshToken = new RefreshToken
        {
            Token = "refreshtoken123",
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        await _authService.RevokeRefreshTokenAsync("refreshtoken123");

        // Assert
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.Token == "refreshtoken123"
        );
        Assert.NotNull(token);
        Assert.True(token.Expires < DateTime.UtcNow);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ValidEmail_GeneratesToken()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        await _authService.RequestPasswordResetAsync("test@example.com");

        // Assert
        var userWithToken = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == "test@example.com"
        );
        Assert.NotNull(userWithToken);
        Assert.NotNull(userWithToken.PasswordResetToken);
        Assert.NotNull(userWithToken.PasswordResetTokenExpires);
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_UpdatesPassword()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
            PasswordResetToken = "resetter123",
            PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1),
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var dto = new PasswordResetRequestDto
        {
            Token = "resettoken123",
            NewPassword = "NewPassword123!",
        };

        // Act
        await _authService.ResetPasswordAsync(dto);

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedUser.PasswordHash);
        Assert.NotNull(updatedUser.PasswordSalt);
        Assert.Null(updatedUser.PasswordResetToken);
        Assert.Null(updatedUser.PasswordResetTokenExpires);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ValidToken_UpdatesEmailConfirmed()
    {
        // Arrange
        var user = new User
        {
            EmailConfirmationToken = "confirmtoken123",
            EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24),
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        await _authService.ConfirmEmailAsync("confirmtoken123");

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.EmailConfirmed);
        Assert.Null(updatedUser.EmailConfirmationToken);
        Assert.Null(updatedUser.EmailConfirmationTokenExpires);
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidToken_ThrowsException()
    {
        // Arrange
        var user = new User
        {
            EmailConfirmationToken = "confirmtoken123",
            EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24),
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _authService.ConfirmEmailAsync("invalidtoken")
        );
    }

    [Fact]
    public async Task RequestEmailConfirmationAsync_ValidEmail_UpdatesToken()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        await _authService.RequestEmailConfirmationAsync("test@example.com");

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == "test@example.com"
        );
        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedUser.EmailConfirmationToken);
        Assert.NotNull(updatedUser.EmailConfirmationTokenExpires);
    }
}
