using Microsoft.EntityFrameworkCore;
using Azure_sql_Assignment.Models;
namespace Azure_sql_Assignment.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; } = null!;
    }
}
