using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace TestContainers.PlayGround.Tests;

/// <summary>
/// Web application factory is once created per class fixture. Therefore, this is the right place to set up the container and database.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global - Instantiated by xUnit
public class IntegrationTestWebApplicationFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
    where TDbContext : DbContext
{
    public MsSqlContainer Container { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            services.Remove(dbContextDescriptor);

            services.AddDbContext<TDbContext>(options => { options.UseSqlServer(Container.GetConnectionString()); });
        });

        builder.UseEnvironment("Development");
    }

    public async Task InitializeAsync()
    {
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPassword("MyPass@word")
            .WithPortBinding(1433, 1433)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();        
        
        await Container.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}