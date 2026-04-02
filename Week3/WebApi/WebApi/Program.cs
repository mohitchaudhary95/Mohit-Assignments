using BookStoreAPI.Data;
using BookStoreAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// 1. Register Services
// ─────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// In-Memory EF Core Database (swap with SQL Server / PostgreSQL in production)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BookStoreDb"));

// Register custom services
builder.Services.AddScoped<IBookService, BookService>();

// ─────────────────────────────────────────────────────────────
// 2. Swagger / OpenAPI Configuration
// ─────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "📚 Book Store API",
        Version = "v1",
        Description = "A RESTful Web API for managing a Book Store inventory. " +
                      "Supports full CRUD operations, search, and filtering.",
        Contact = new OpenApiContact
        {
            Name = "Book Store Dev Team",
            Email = "dev@bookstore.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Enable XML comments in Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // Group endpoints by controller
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
});

// ─────────────────────────────────────────────────────────────
// 3. Build App
// ─────────────────────────────────────────────────────────────
var app = builder.Build();

// Seed in-memory DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ─────────────────────────────────────────────────────────────
// 4. Middleware Pipeline
// ─────────────────────────────────────────────────────────────

// Enable Swagger in all environments (restrict to Development in production apps)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API v1");
    options.RoutePrefix = string.Empty;          // Swagger UI at root: https://localhost:PORT/
    options.DocumentTitle = "📚 Book Store API";
    options.DisplayRequestDuration();            // Show request time in UI
    options.EnableTryItOutByDefault();           // Auto-enable "Try it out"
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
