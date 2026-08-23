using LinkShortener.Data;
using LinkShortener.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string dbConnectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");

// SQLite creates the database file if it's missing but not its folder. home/data won't exist on a new App Service instance
string dbDirectory = Path.GetDirectoryName(new SqliteConnectionStringBuilder(dbConnectionString).DataSource) ?? string.Empty;

if (dbDirectory.Length > 0)
{
    Directory.CreateDirectory(dbDirectory);
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(dbConnectionString));

builder.Services.AddScoped<ILinkService, LinkService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply any pending migrations at startup
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serves wwwroot/index.html at "/"
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
