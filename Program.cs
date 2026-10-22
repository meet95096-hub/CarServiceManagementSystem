using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Detect and configure database provider
var configuration = builder.Configuration;
var preferredProvider = configuration.GetValue<string>("DatabaseProvider") ?? "Auto";
var sqlServerConn = configuration.GetConnectionString("SqlServerConnection");
var sqliteConn = configuration.GetConnectionString("SqliteConnection") ?? "Data Source=CarService.db";

string activeProvider = "Sqlite";

if (preferredProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) || preferredProvider.Equals("Auto", StringComparison.OrdinalIgnoreCase))
{
    // Test if SQL Server is accessible
    bool sqlServerAvailable = false;
    if (!string.IsNullOrWhiteSpace(sqlServerConn))
    {
        try
        {
            var csb = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(sqlServerConn)
            {
                ConnectTimeout = 1
            };
            using var testConn = new Microsoft.Data.SqlClient.SqlConnection(csb.ConnectionString);
            testConn.Open();
            sqlServerAvailable = true;
            testConn.Close();
        }
        catch
        {
            sqlServerAvailable = false;
        }
    }

    if (sqlServerAvailable)
    {
        activeProvider = "SqlServer";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(sqlServerConn));
    }
    else
    {
        activeProvider = "Sqlite";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(sqliteConn));
    }
}
else
{
    activeProvider = "Sqlite";
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(sqliteConn));
}

// Register database info service for the UI to display connection details
builder.Services.AddSingleton(new DatabaseStatusInfo(activeProvider, activeProvider == "SqlServer" ? sqlServerConn! : sqliteConn));

var app = builder.Build();

// Auto initialize and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        await DbSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
