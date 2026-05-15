using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Application.Services;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Tests.Application;

public class AuthServiceTests
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] =
                        "TestJwtSecurityKey1234567890123456789012345678901234567890123456789012345678901234567890",
                    ["Jwt:Issuer"] = "test-issuer",
                    ["Jwt:Audience"] = "test-audience",
                }
            )
            .Build();

        _mapper = new FakeMapper();
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_CreatesUserAndReturnsDto()
    {
        await using var context = CreateContext();
        var service = new AuthService(_mapper, _configuration, context);

        var dto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
        };

        var response = await service.RegisterAsync(dto);

        Assert.NotNull(response);
        Assert.Equal(dto.Username, response.Username);
        Assert.Equal(dto.Email, response.Email);
        Assert.True(response.Id > 0);
        Assert.NotEqual(default, response.CreatedAt);

        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        Assert.NotNull(createdUser);
        Assert.Equal(dto.Email, createdUser!.Email);
        Assert.False(createdUser.EmailConfirmed);
        Assert.False(string.IsNullOrWhiteSpace(createdUser.EmailConfirmationToken));
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ThrowsApplicationException()
    {
        await using var context = CreateContext();
        var service = new AuthService(_mapper, _configuration, context);

        var dto = new UserRegisterDto
        {
            Username = "duplicate",
            Email = "first@example.com",
            Password = "Password123!",
        };

        await service.RegisterAsync(dto);

        var duplicateDto = new UserRegisterDto
        {
            Username = "duplicate",
            Email = "second@example.com",
            Password = "Password123!",
        };

        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            service.RegisterAsync(duplicateDto)
        );

        Assert.Contains("Username already exists", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithConfirmedEmail_ReturnsTokensAndPersistsRefreshToken()
    {
        await using var context = CreateContext();
        var service = new AuthService(_mapper, _configuration, context);

        var registerDto = new UserRegisterDto
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "Password123!",
        };

        await service.RegisterAsync(registerDto);

        var user = await context.Users.FirstAsync(u => u.Username == registerDto.Username);
        user.EmailConfirmed = true;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        var loginDto = new UserLoginDto { Username = "loginuser", Password = "Password123!" };

        var (accessToken, refreshToken) = await service.LoginAsync(loginDto);

        Assert.False(string.IsNullOrWhiteSpace(accessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));

        var savedToken = await context.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.Token == refreshToken
        );
        Assert.NotNull(savedToken);
        Assert.Equal(user.Id, savedToken!.UserId);
    }

    [Fact]
    public async Task LoginWithRefreshAsync_RevokesOldTokenAndReturnsNewTokens()
    {
        await using var context = CreateContext();
        var service = new AuthService(_mapper, _configuration, context);

        var registerDto = new UserRegisterDto
        {
            Username = "refreshuser",
            Email = "refresh@example.com",
            Password = "Password123!",
        };

        await service.RegisterAsync(registerDto);

        var user = await context.Users.FirstAsync(u => u.Username == registerDto.Username);
        user.EmailConfirmed = true;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        var loginDto = new UserLoginDto { Username = "refreshuser", Password = "Password123!" };

        var (_, refreshToken) = await service.LoginAsync(loginDto);
        var result = await service.LoginWithRefreshAsync(refreshToken);

        Assert.False(string.IsNullOrWhiteSpace(result.accessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.refreshToken));
        Assert.NotEqual(refreshToken, result.refreshToken);

        var oldToken = await context.RefreshTokens.FirstAsync(rt => rt.Token == refreshToken);
        Assert.True(oldToken.Expires <= DateTime.UtcNow);
    }

    [Fact]
    public async Task ConfirmEmailAsync_WithValidToken_SetsEmailConfirmed()
    {
        await using var context = CreateContext();
        var service = new AuthService(_mapper, _configuration, context);

        var registerDto = new UserRegisterDto
        {
            Username = "confirmuser",
            Email = "confirm@example.com",
            Password = "Password123!",
        };

        await service.RegisterAsync(registerDto);
        var user = await context.Users.FirstAsync(u => u.Username == registerDto.Username);

        var token = user.EmailConfirmationToken;
        Assert.False(string.IsNullOrWhiteSpace(token));

        await service.ConfirmEmailAsync(token!);

        var confirmedUser = await context.Users.FirstAsync(u => u.Username == registerDto.Username);
        Assert.True(confirmedUser.EmailConfirmed);
        Assert.Null(confirmedUser.EmailConfirmationToken);
        Assert.Null(confirmedUser.EmailConfirmationTokenExpires);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}

internal class FakeMapper : IMapper
{
    public AutoMapper.IConfigurationProvider ConfigurationProvider =>
        throw new NotSupportedException();

    public object Map(object source, Type sourceType, Type destinationType)
    {
        if (
            source is global::Core.Entities.User user
            && destinationType == typeof(global::Core.DTOs.UserResponseDto)
        )
        {
            return new global::Core.DTOs.UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
            };
        }

        throw new NotSupportedException();
    }

    public object Map(object source, object destination, Type sourceType, Type destinationType) =>
        throw new NotSupportedException();

    public object Map(
        object source,
        Type sourceType,
        Type destinationType,
        Action<IMappingOperationOptions<object, object>> opts
    ) => Map(source, sourceType, destinationType);

    public object Map(
        object source,
        object destination,
        Type sourceType,
        Type destinationType,
        Action<IMappingOperationOptions<object, object>> opts
    ) => Map(source, destination, sourceType, destinationType);

    public TDestination Map<TDestination>(object source)
    {
        if (
            source is global::Core.Entities.User user
            && typeof(TDestination) == typeof(global::Core.DTOs.UserResponseDto)
        )
        {
            return (TDestination)
                (object)
                    new global::Core.DTOs.UserResponseDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                    };
        }

        throw new NotSupportedException();
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return Map<TDestination>((object)source!);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        throw new NotSupportedException();
    }

    public TDestination Map<TDestination>(
        object source,
        Action<IMappingOperationOptions<object, TDestination>> opts
    ) => Map<TDestination>(source);

    public TDestination Map<TSource, TDestination>(
        TSource source,
        Action<IMappingOperationOptions<TSource, TDestination>> opts
    ) => Map<TSource, TDestination>(source);

    public TDestination Map<TSource, TDestination>(
        TSource source,
        TDestination destination,
        Action<IMappingOperationOptions<TSource, TDestination>> opts
    ) =>
        Map(source!, destination!, typeof(TSource), typeof(TDestination)) is TDestination casted
            ? casted
            : throw new NotSupportedException();

    public IQueryable<TDestination> ProjectTo<TDestination>(
        IQueryable source,
        object parameters,
        params Expression<Func<TDestination, object>>[] membersToExpand
    ) => throw new NotSupportedException();

    public IQueryable<TDestination> ProjectTo<TDestination>(
        IQueryable source,
        IDictionary<string, object> parameters,
        params string[] membersToExpand
    ) => throw new NotSupportedException();

    public IQueryable ProjectTo(
        IQueryable source,
        Type destinationType,
        IDictionary<string, object> parameters,
        params string[] membersToExpand
    ) => throw new NotSupportedException();
}
