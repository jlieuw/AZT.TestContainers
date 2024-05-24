namespace TestContainers.ApiApp.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using TestContainers.ApiApp.Infrastructure;

internal class PlaygroundApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public PlaygroundApplication(string environment = "Development")
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://localhost:2375");

            var container = new MsSqlBuilder().Build();

            container.StartAsync().Wait();

            services.AddScoped(sp =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(container.GetConnectionString())
                    .UseApplicationServiceProvider(sp)
                    .Options;

                // Apply migrations
                using (var context = new AppDbContext(options))
                {
                    context.Database.Migrate();
                }

                return options;
            });
        });

        return base.CreateHost(builder);
    }
}
