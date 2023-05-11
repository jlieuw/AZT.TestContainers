namespace TestContainers.IntegratedTest.Fixtures;

using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Xunit;

public sealed class DemoAppImage : IImage, IAsyncLifetime
{
    public const int HttpsPort = 443;

    public const string CertificateFilePath = "certificate.pfx";

    public const string CertificatePassword = "testcontainers";

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private readonly IImage _image = new DockerImage(string.Empty, "testcontainers-demoapp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

    public async Task InitializeAsync()
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            await new ImageFromDockerfileBuilder()
                .WithName(this)
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile("Dockerfile")
                .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
                .WithDeleteIfExists(false)
                .Build()
                .CreateAsync();
        }
        finally
        {
            _ = _semaphoreSlim.Release();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public string Repository => _image.Repository;

    public string Name => _image.Name;

    public string Tag => _image.Tag;

    public string FullName => _image.FullName;

    public string GetHostname() => _image.GetHostname();
}
