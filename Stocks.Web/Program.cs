using FluentMigrator.Runner;
using Stocks.DataAccess;
using Stocks.DataAccess.Authentication;
using Stocks.DataAccess.Migration.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Routing
{
    builder.Services.AddRazorPages(options => {
        options.Conventions.AddPageRoute("/Authentication/Login", "login");
        options.Conventions.AddPageRoute("/Authentication/Register", "register");
    });
}

// add services to DI container
{
    var services = builder.Services;
    services.AddCors();
}

// DB config
var connectionString = builder.Configuration.GetConnectionString("Default");
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Please define a connection string in your config.");
    }

    builder.Services.AddTransient(x => new DatabaseConnection(connectionString));
    builder.Services.AddTransient(typeof(ConnectionChecker));
    builder.Services.AddTransient(typeof(AuthenticationRepository));

    // Migrations runner config
    builder.Services.AddFluentMigratorCore().ConfigureRunner(r => r
        // .AddMySql5()
        .AddSQLite()
        // Migrations connection string
        .WithGlobalConnectionString(connectionString)
        // Assembly we check for migrations 
        .ScanIn(typeof(AddUsersTable_001).Assembly).For.Migrations())
        .AddLogging(l => l.AddFluentMigratorConsole());
};

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Run migrations
using (var migratorScope = app.Services.CreateScope())
{
    var services = migratorScope.ServiceProvider;
    var migrationRunner = services.GetService<IMigrationRunner>();
    migrationRunner.ValidateVersionOrder();
    migrationRunner.MigrateUp();
}

await app.RunAsync();
