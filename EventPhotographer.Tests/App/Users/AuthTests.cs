using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace EventPhotographer.Tests.App.Users;

internal class AuthTests : BaseIntegrationTest
{
    public AuthTests(AppWebApplicationFactory factory) : base(factory)
    {
    }

    public async Task Register_ShouldCreateUser()
    {
        // Arrange
        var initialUserCount = await Db.Users.CountAsync();
        var requestData = new
        {
            Name = "Test test",
            Email = "test@test.com",
            Password = "Secret",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", requestData);

        // Assert
        Assert.Equal(initialUserCount + 1, await Db.Users.CountAsync());
    }


}
