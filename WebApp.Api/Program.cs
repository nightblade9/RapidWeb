using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Api;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Configure CORS access to allow Blazor clients
        var corsPolicy = "_AllowBlazorClient";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy, policy =>
            {
                policy.WithOrigins("https://localhost:7252") // Adjust if your Blazor app runs on a different port
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddControllers();

        // Add JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };

            // Extract JWT from cookie instead of the Authorization header
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Check if the request contains the ApiAuthToken cookie
                    if (context.Request.Cookies.ContainsKey("ApiAuthToken"))
                    {
                        context.Token = context.Request.Cookies["ApiAuthToken"];
                    }
                    return Task.CompletedTask;
                }
            };
        });


        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Enable Authentication and Authorization Middleware
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Allow CORS for localhost
        app.UseCors(corsPolicy);

        app.Run();
    }
}