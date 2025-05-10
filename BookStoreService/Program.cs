using BookStore_Domain.Models;
using BookStore_Domain.Repositories;
using BookStore_Domain.Seeders;
using Microsoft.EntityFrameworkCore;
using BookStore_Application.Services;
using BookStore_Domain;

namespace BookStoreService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"), ServiceLifetime.Transient);
        builder.Services.AddScoped<IBook_Repository, Book_Repository>();
        builder.Services.AddScoped<ICategory_Repository, Category_Repository>();

        builder.Services.AddScoped<IBook_Service, Book_Service>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMemoryCache();

        builder.Services.AddScoped<IBookStoreSeeder, BookStoreSeeder>();
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
        var seeder = scope.ServiceProvider.GetRequiredService<IBookStoreSeeder>();
        await seeder.Seed();

        app.Run();
    }
} 