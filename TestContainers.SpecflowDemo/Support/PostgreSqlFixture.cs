namespace TestContainers.SpecflowDemo.Support;

using System.Threading.Tasks;
using Npgsql;
using Testcontainers.PostgreSql;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; }

    public NpgsqlConnection? Connection { get; private set; }

    public PostgreSqlFixture()
    {
        Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://localhost:2375");

        var testcontainersBuilder =
            new PostgreSqlBuilder()
            .WithDatabase("db")
            .WithUsername("db_user")
            .WithPassword("db_password");

        Container = testcontainersBuilder.Build();
    }

    public async Task UseBackupFile(byte[] backupFile)
    {
        await Container.CopyAsync(backupFile, "/tmp/db_backup.dump");

        var command = "pg_restore --username=db_user --dbname=db -1 /tmp/db_backup.dump";

        _ = await Container.ExecAsync(command.Split(' '));
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        Connection = new NpgsqlConnection(Container.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        if (Connection is not null)
        {
            await Connection.DisposeAsync();
        }

        await Container.DisposeAsync();
    }
}
