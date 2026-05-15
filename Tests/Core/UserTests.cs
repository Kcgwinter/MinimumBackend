using System;
using Core.Entities;

namespace Tests.Core;

public class UserTests
{
    [Fact]
    public void Create_UserTest()
    {
        //arrange
        string Username = "testuser";
        string Email = "test@example.com";
        string Password = "password";

        //act
        var user = new User()
        {
            Username = Username,
            Email = Email,
            PasswordHash = System.Text.Encoding.UTF8.GetBytes(Password),
            PasswordSalt = System.Text.Encoding.UTF8.GetBytes("salt"),
        };
        //assert
        Assert.Equal(Username, user.Username);
        Assert.Equal(Email, user.Email);
        Assert.Equal(Password, System.Text.Encoding.UTF8.GetString(user.PasswordHash));
        Assert.Equal("salt", System.Text.Encoding.UTF8.GetString(user.PasswordSalt));
    }
}
