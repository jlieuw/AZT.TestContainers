namespace TestContainers.IntegratedTest.Fixtures;

using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

public sealed class DemoAppContainer : HttpClient, IAsyncLifetime
{
    private static readonly X509Certificate Certificate = new X509Certificate2(DemoAppImage.CertificateFilePath, DemoAppImage.CertificatePassword);

    private static readonly DemoAppImage Image = new();

    private readonly INetwork _demoAppNetwork;

    private readonly PostgreSqlContainer _postgresqlContainer;

    private readonly IContainer _demoAppContainer;

    public DemoAppContainer()
      : base(new HttpClientHandler
      {
          // Trust the development certificate.
          ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
      })
    {
        Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://localhost:2375");

        const string demoAppStorage = "demoAppStorage";

        _demoAppNetwork = new NetworkBuilder()
          .WithName(Guid.NewGuid().ToString("D"))
          .Build();

        _postgresqlContainer =
            new PostgreSqlBuilder()
            .WithDatabase("db")
            .WithUsername("db_user")
            .WithPassword("db_password")
            .WithNetwork(_demoAppNetwork)
            .WithNetworkAliases(demoAppStorage)
            .WithPortBinding(PostgreSqlBuilder.PostgreSqlPort, false)
            .Build();

        var connectionString = $"Server={demoAppStorage};Database=db;User Id=db_user;Password=db_password;";

        _demoAppContainer = new ContainerBuilder()
            .WithImage(Image)
            .WithNetwork(_demoAppNetwork)
            .WithPortBinding(DemoAppImage.HttpsPort, true)
            .WithEnvironment("ASPNETCORE_URLS", "https://+")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", DemoAppImage.CertificateFilePath)
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", DemoAppImage.CertificatePassword)
            .WithEnvironment("ConnectionStrings__StoreConnectionString", connectionString)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(DemoAppImage.HttpsPort))
            .Build();
    }

    public async Task InitializeAsync()
    {
        // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
        await Image.InitializeAsync();

        await _demoAppNetwork.CreateAsync();

        await _postgresqlContainer.StartAsync();

        await UseDatabaseBackup();

        await _demoAppContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Image.DisposeAsync();

        await _demoAppContainer.DisposeAsync();

        await _postgresqlContainer.DisposeAsync();

        await _demoAppNetwork.DeleteAsync();
    }

    public void SetBaseAddress()
    {
        try
        {
            UriBuilder uriBuilder = new("https", _demoAppContainer.Hostname, _demoAppContainer.GetMappedPublicPort(DemoAppImage.HttpsPort));
            BaseAddress = uriBuilder.Uri;
        }
        catch
        {
            // Set the base address only once.
        }
    }

    public async Task UseDatabaseBackup()
    {
        await _postgresqlContainer.CopyFileAsync("/tmp/db_backup.dump", await File.ReadAllBytesAsync("Fixtures/db_backup.dump"));

        var command = "pg_restore --username=db_user --dbname=db -1 /tmp/db_backup.dump";

        _ = await _postgresqlContainer.ExecAsync(command.Split(' '));
    }
}
