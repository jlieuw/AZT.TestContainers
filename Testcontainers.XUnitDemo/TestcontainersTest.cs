namespace Testcontainers.XUnitDemo;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TestContainers.Shared.Contexts;
using TestContainers.Shared.Services;

public class TestcontainersTest
{
    [Fact]
    public async Task Removing_Stale_Accounts_Should_Remove_All_Stale_Accounts()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://localhost:2375");

        var testcontainersBuilder =
            new PostgreSqlBuilder()
            .WithDatabase("db")
            .WithUsername("db_user")
            .WithPassword("db_password");

        await using var container = testcontainersBuilder.Build();

        await container.StartAsync();

        await container.CopyAsync(await File.ReadAllBytesAsync("db_backup.dump"), "/tmp/db_backup.dump");

        var command = "pg_restore --username=db_user --dbname=db -1 /tmp/db_backup.dump";

        _ = await container.ExecAsync(command.Split(' '));

        StoreContext storeContext = new(
            new DbContextOptionsBuilder<StoreContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options);

        AccountService service = new(storeContext);

        // Act
        var numberOfAccounts = service.GetAllAccounts().Count();
        var numberOfRemovedAccounts = service.RemoveAllStaleAccounts();

        // Assert
        _ = (numberOfAccounts - numberOfRemovedAccounts).Should().Be(852);
    }
}
