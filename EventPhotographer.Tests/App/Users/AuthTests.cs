using EventPhotographer.App.Users.Dto;
using EventPhotographer.App.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace EventPhotographer.Tests.App.Users;

public class AuthTests : BaseIntegrationTest
{
    public AuthTests(AppWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_ShouldCreateUser()
    {
        // Arrange
        var initialUserCount = await Db.Users.CountAsync();
        var requestData = new
        {
            Name = "Test test",
            Email = "test@test.com",
            Password = "Secret123",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", requestData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(initialUserCount + 1, await Db.Users.CountAsync());
    }

    [Fact]
    public async Task Register_ShouldNotCreateUserWithSameEmail()
    {
        // Arrange
        var initialUser = new User { Email = "test@test.com" };
        var result = await UserManager.CreateAsync(initialUser);

        var initialUserCount = await Db.Users.CountAsync();
        var requestData = new
        {
            Name = "Test test",
            Email = "test@test.com",
            Password = "Secret123",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", requestData);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(initialUserCount, await Db.Users.CountAsync());
    }

    [Fact]
    public async Task GetUser_ShouldGetLoggedInUser()
    {
        var client = await GetClientWithAuth(new User
        {
            Name = "Test User",
            Email = "test@test.com"
        });

        var response = await client.GetAsync("/api/auth/user");
        var deserialized = await response.Content.ReadFromJsonAsync<UserLoginResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(deserialized);
        Assert.Equal("Test User", deserialized.Name);
        Assert.Equal("test@test.com", deserialized.Email);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorizedIfUserIsNotLoggedIn()
    {
        var response = await Client.GetAsync("/api/auth/user");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
