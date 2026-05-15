using Core.DTOs;
using Core.Entities;
using Moq;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public async Task FirstTest()
    {
        {
            //arrange
            var num = 5;

            //act
            num = num + num;

            //assert
            Assert.Equal(10, num);
        }
    }

    [Fact]
    public void Create_UserTest()
    {
        //arrange
        var user = new UserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password",
        };

        //act
        //assert
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
    }
}
