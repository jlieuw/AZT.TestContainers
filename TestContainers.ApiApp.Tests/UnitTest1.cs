namespace TestContainers.ApiApp.Tests;

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Testcontainers.MsSql;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{

    [Fact]
    public async Task Test1()
    {
        // Arrange
        await using var application = new PlaygroundApplication();
        using var client = application.CreateClient();

        var request = "/employees";

        // Act
        var response = await client.GetAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        // deserialize the response ignoring case
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var employees = JsonSerializer.Deserialize<List<Employee>>(responseString, options);

        employees.Should().NotBeNull();
    }
}
