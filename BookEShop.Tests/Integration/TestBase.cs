using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookEShop.Domain;
using BookEShop.Domain.Seeders;

namespace BookEShop.Tests.Integration;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;
    protected readonly DataContext _context;
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

        _client = _factory.CreateClient();

        // Create a new scope for the context
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DataContext>();
        _context.Database.EnsureCreated();
    }

    public async Task InitializeAsync()
    {
        // Clear the database before each test
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        // Seed the database with test data
        var seeder = new BookEShopSeeder(_context);
        await seeder.Seed();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
        await _factory.DisposeAsync();
    }
} 