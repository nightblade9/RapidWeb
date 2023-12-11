using System.Text.Json.Serialization;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Routing
{
    builder.Services.AddRazorPages(options => {
        options.Conventions.AddPageRoute("/Index", "index");
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
{
    builder.Services.AddTransient(x =>
        new MySqlConnection(builder.Configuration.GetConnectionString("Default")));
}

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

app.Run();
