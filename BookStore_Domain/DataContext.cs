using Microsoft.EntityFrameworkCore;
using BookStore_Domain.Models;

namespace BookStore_Domain;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Book_Category)
            .WithMany()
            .HasForeignKey("CategoryId");

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Book)
            .WithMany()
            .HasForeignKey(oi => oi.BookId);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.ShoppingCart)
            .WithMany(sc => sc.CartItems)
            .HasForeignKey(ci => ci.ShoppingCartId);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Book)
            .WithMany()
            .HasForeignKey(ci => ci.BookId);
    }
} 