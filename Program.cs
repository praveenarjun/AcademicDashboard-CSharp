using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

// Make the Program class async
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        // Configure MySQL database connection
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDatabaseServices(connectionString);

        // Enable sensitive data logging in development
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.Configure<DbContextOptionsBuilder>(options =>
            {
                options.EnableSensitiveDataLogging();
            });
        }

        // Add session support
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        // Initialize and seed the database
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Initializing database...");

            await DbInitializer.InitializeAsync(app.Services, logger);

            logger.LogInformation("Database initialization completed successfully.");
            Console.WriteLine("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
            Console.WriteLine($"Database initialization error: {ex.Message}");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        // Map static assets if the method exists (for compatibility)
        if (app.GetType().GetMethod("MapStaticAssets") != null)
        {
            app.GetType().GetMethod("MapStaticAssets").Invoke(app, null);
        }

        Console.WriteLine("Application started. Press Ctrl+C to shut down.");
        await app.RunAsync();
    }
}
