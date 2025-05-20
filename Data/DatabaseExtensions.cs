using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using System;

namespace StudentEnrollmentSystem1.Data
{
    public static class DatabaseExtensions
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<T>>();

                try
                {
                    var db = services.GetRequiredService<T>();
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(T).Name);

                    db.Database.Migrate();

                    // Seed data if needed
                    if (db is ApplicationDbContext dbContext)
                    {
                        Models.CourseData.SeedCourses(dbContext);
                        Models.CourseOfferingData.SeedCourseOfferings(dbContext);
                    }

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(T).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return host;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, string connectionString)
        {
            // Register the DbContext with MySQL
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                // Get the logger factory from the service provider
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );

                        // Set command timeout
                        mySqlOptions.CommandTimeout(60);

                        // Enable detailed errors (not supported for MySQL, removing this line)
                    }
                );

                // Pass the logger factory to the DbContext
                options.UseLoggerFactory(loggerFactory);

                // Enable sensitive data logging in development
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            // Register repositories
            services.AddScoped<StudentRepository>();
            services.AddScoped<CourseRepository>();
            services.AddScoped<CourseOfferingRepository>();
            services.AddScoped<CourseEnrollmentRepository>();
            services.AddScoped<PaymentRepository>();
            services.AddScoped<StudentEnquiryRepository>();
            services.AddScoped<TeacherEvaluationRepository>();
            services.AddScoped<TeacherRepository>();
            services.AddScoped<StudentBankDetailsRepository>();
            // services.AddScoped<TimetableRepository>();

            return services;
        }
    }
}