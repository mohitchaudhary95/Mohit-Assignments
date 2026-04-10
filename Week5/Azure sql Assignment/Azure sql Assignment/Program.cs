using Azure_sql_Assignment.Data;
using Microsoft.EntityFrameworkCore;

namespace Azure_sql_Assignment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Get connection string properly
            var connectionString = builder.Configuration.GetConnectionString("AzureSqlConnection");

            // ❗ Add null check to avoid runtime error
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string 'AzureSqlConnection' not found.");
            }

            // ✅ Register DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ✅ Add MVC services
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ✅ Middleware pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // ❗ REQUIRED (instead of MapStaticAssets if causing error)

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}