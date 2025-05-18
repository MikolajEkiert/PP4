using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory;
using BookEShop.Domain;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using BookEShop.Application.Services;
using BookEShop.Application.Interfaces;
using BookEShop.Domain.Seeders;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"), ServiceLifetime.Transient);
        // Repositories
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        // Services
        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMemoryCache();

        builder.Services.AddScoped<IBookEShopSeeder, BookEShopSeeder>();
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IBookEShopSeeder>();
        await seeder.Seed();

        app.Run();
    }
}
