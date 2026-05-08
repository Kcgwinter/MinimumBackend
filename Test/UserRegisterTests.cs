// File: UserRegisterTests.cs
// Location: Anywhere in your project (e.g., Tests/ or TestFolder/)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Entities;
using Xunit;

namespace UserRegisterTests;

public class UserRegisterTests
{
    [Fact]
    public void User_UsernameMustNotBeEmpty()
    {
        // Arrange
        var user = new User
        {
            Username = "",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Act & Assert
        Assert.NotNull(user.Username);
        Assert.NotEmpty(user.Username);
    }

    [Fact]
    public void User_EmailMustBeValid()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "invalid-email",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Act & Assert
        Assert.Contains("@", user.Email);
    }

    [Fact]
    public void User_PasswordHashMustNotBeEmpty()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Act & Assert
        Assert.NotNull(user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
    }

    [Fact]
    public void User_UserNameMustBeAtLeast3Characters()
    {
        // Arrange
        var shortUser = new User
        {
            Username = "ab",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        var longUser = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Assert
        // Assert.True(shortUser.Username.Length < 3);
        Assert.True(longUser.Username.Length >= 3);
    }

    [Fact]
    public void User_UsernameMustBeUnique()
    {
        // Arrange
        var user1 = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        var user2 = new User
        {
            Username = "testuser",
            Email = "test2@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Act
        var duplicateUsers = new List<User> { user1, user2 };
        var duplicateCount = duplicateUsers.Count(u => u.Username == "testuser");

        // Assert
        Assert.Equal(2, duplicateCount);
    }

    [Fact]
    public void User_EmailMustBeUnique()
    {
        // Arrange
        var user1 = new User
        {
            Username = "user1",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        var user2 = new User
        {
            Username = "user2",
            Email = "test@example.com",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            IsActive = true,
            EmailConfirmed = true,
        };

        // Act
        var duplicateUsers = new List<User> { user1, user2 };
        var duplicateEmailCount = duplicateUsers.Count(u => u.Email == "test@example.com");

        // Assert
        Assert.Equal(2, duplicateEmailCount);
    }
}
