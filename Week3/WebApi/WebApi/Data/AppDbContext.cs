using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookStoreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed some sample data
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Genre = "Programming",
                    Price = 39.99m,
                    Stock = 25,
                    PublishedDate = new DateTime(2008, 8, 1),
                    IsAvailable = true
                },
                new Book
                {
                    Id = 2,
                    Title = "The Pragmatic Programmer",
                    Author = "David Thomas",
                    Genre = "Programming",
                    Price = 45.00m,
                    Stock = 18,
                    PublishedDate = new DateTime(1999, 10, 20),
                    IsAvailable = true
                },
                new Book
                {
                    Id = 3,
                    Title = "Design Patterns",
                    Author = "Gang of Four",
                    Genre = "Software Engineering",
                    Price = 54.99m,
                    Stock = 10,
                    PublishedDate = new DateTime(1994, 10, 31),
                    IsAvailable = true
                },
                new Book
                {
                    Id = 4,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Genre = "Fiction",
                    Price = 12.99m,
                    Stock = 50,
                    PublishedDate = new DateTime(1925, 4, 10),
                    IsAvailable = true
                },
                new Book
                {
                    Id = 5,
                    Title = "Atomic Habits",
                    Author = "James Clear",
                    Genre = "Self-Help",
                    Price = 18.00m,
                    Stock = 0,
                    PublishedDate = new DateTime(2018, 10, 16),
                    IsAvailable = false
                }
            );
        }
    }
}
