using Respawn;
using TestContainers.Playground.Core;

namespace TestContainers.PlayGround.Tests;

/// <summary>
/// Base class for integration tests. Resets the database before each test, because xUnit instantiates the test class for each test.
/// </summary>
public abstract class AspnetCoreIntegrationFixture(IntegrationTestWebApplicationFactory<Program, AppDbContext> factory)
    : IClassFixture<IntegrationTestWebApplicationFactory<Program, AppDbContext>>, IAsyncLifetime
{
    protected IntegrationTestWebApplicationFactory<Program, AppDbContext> Factory { get; } = factory;

    private Respawner databaseReset = null!;

    public async Task InitializeAsync()
    {
        databaseReset = await Respawner.CreateAsync(Factory.Container.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await databaseReset.ResetAsync(Factory.Container.GetConnectionString());
    }
}