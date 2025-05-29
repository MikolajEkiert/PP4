using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookEShop.Domain;
using BookEShop.Domain.Seeders;
using Xunit;

namespace BookEShop.Test.Integration;

public abstract class TestBase : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient Client;
    protected DataContext Context;
    private readonly string _dbName;

    protected TestBase()
    {
        _dbName = $"TestDb_{Guid.NewGuid()}";
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<DataContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseInMemoryDatabase(_dbName);
                    });
                });
            });

        Client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Create a new scope for the context
        var scope = _factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Ensure database is created and clean
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();

        // Seed the database with test data
        var seeder = new BookEShopSeeder(Context);
        await seeder.Seed();
    }

    public async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.DisposeAsync();
        }
        await _factory.DisposeAsync();
    }
} 