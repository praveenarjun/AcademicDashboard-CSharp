using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data;
using StudentEnrollmentSystem1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace StudentEnrollmentSystem1.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                logger.LogInformation("Database created or already exists.");

                // Ensure all tables are created
                await EnsureTablesCreatedAsync(context, logger);

                // Skip migrations if tables already exist
                try
                {
                    // Check if the database provider supports migrations
                    var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();

                    // Check if migrations history table exists
                    bool migrationHistoryExists = await context.Database.GetService<IRelationalDatabaseCreator>()
                        .ExistsAsync();

                    // Only apply migrations if the migrations history table exists and there are pending migrations
                    if (migrationHistoryExists)
                    {
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            logger.LogInformation("Applying pending migrations...");
                            await context.Database.MigrateAsync();
                            logger.LogInformation("Migrations applied successfully.");
                        }
                        else
                        {
                            logger.LogInformation("No pending migrations. Database is up to date.");
                        }
                    }
                    else
                    {
                        logger.LogInformation("Migrations history table doesn't exist. Skipping migrations.");

                        // Create migrations history table if it doesn't exist
                        try
                        {
                            await context.Database.ExecuteSqlRawAsync(@"
                                CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                                    `MigrationId` varchar(150) NOT NULL,
                                    `ProductVersion` varchar(32) NOT NULL,
                                    PRIMARY KEY (`MigrationId`)
                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                            ");
                            logger.LogInformation("Created migrations history table.");
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning($"Could not create migrations history table: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Error checking migrations: {ex.Message}. Continuing with initialization.");
                }

                // Seed courses if none exist
                if (!context.Courses.Any())
                {
                    logger.LogInformation("Seeding course data...");
                    await SeedCoursesAsync(context);
                    logger.LogInformation("Course data seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Course data already exists. Skipping seed.");
                }

                // Seed default teacher data
                try
                {
                    // The Teachers table should already be created by EnsureTablesCreatedAsync
                    if (!context.Teachers.Any())
                    {
                        logger.LogInformation("Seeding teacher data...");
                        await SeedTeachersAsync(context);
                        logger.LogInformation("Teacher data seeded successfully.");
                    }
                    else
                    {
                        logger.LogInformation("Teacher data already exists. Skipping seed.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Could not seed teacher data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private static async Task SeedCoursesAsync(ApplicationDbContext context)
        {
            var courses = new[]
            {
                new Course { Name = "BTech", Fee = 120000, Description = "Bachelor of Technology" },
                new Course { Name = "BBA", Fee = 80000, Description = "Bachelor of Business Administration" },
                new Course { Name = "BCA", Fee = 70000, Description = "Bachelor of Computer Applications" },
                new Course { Name = "MBA", Fee = 150000, Description = "Master of Business Administration" },
                new Course { Name = "MCA", Fee = 100000, Description = "Master of Computer Applications" }
            };

            await context.Courses.AddRangeAsync(courses);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTeachersAsync(ApplicationDbContext context)
        {
            try
            {
                // Use direct SQL to insert teachers with INSERT IGNORE to handle duplicates
                await context.Database.ExecuteSqlRawAsync(@"
                    INSERT IGNORE INTO Teachers 
                    (TeacherId, TeacherName, Email, Password, Department, RegistrationDate) 
                    VALUES 
                    ('T001', 'John Smith', 'john.smith@example.com', 'Password123', 'Computer Science', NOW()),
                    ('T002', 'Jane Doe', 'jane.doe@example.com', 'Password123', 'Business Administration', NOW())
                ");
            }
            catch (Exception)
            {
                // If direct SQL fails, try the EF Core approach
                try
                {
                    var teachers = new[]
                    {
                        new Teacher {
                            TeacherId = "T001",
                            TeacherName = "John Smith",
                            Email = "john.smith@example.com",
                            Password = "Password123",
                            Department = "Computer Science"
                        },
                        new Teacher {
                            TeacherId = "T002",
                            TeacherName = "Jane Doe",
                            Email = "jane.doe@example.com",
                            Password = "Password123",
                            Department = "Business Administration"
                        }
                    };

                    // For each teacher, check if it exists before adding
                    foreach (var teacher in teachers)
                    {
                        var existingTeacher = await context.Teachers
                            .FirstOrDefaultAsync(t => t.TeacherId == teacher.TeacherId || t.Email == teacher.Email);

                        if (existingTeacher == null)
                        {
                            await context.Teachers.AddAsync(teacher);
                        }
                    }

                    await context.SaveChangesAsync();
                }
                catch
                {
                    // Ignore any errors during seeding
                }
            }
        }

        private static async Task EnsureTablesCreatedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Create Students table if it doesn't exist
                await CreateStudentsTableIfNotExistsAsync(context, logger);

                // Create Courses table if it doesn't exist
                await CreateCoursesTableIfNotExistsAsync(context, logger);

                // Create Teachers table if it doesn't exist
                await CreateTeachersTableIfNotExistsAsync(context, logger);

                // Create CourseEnrollments table if it doesn't exist
                await CreateCourseEnrollmentsTableIfNotExistsAsync(context, logger);

                // Create Payments table if it doesn't exist
                await CreatePaymentsTableIfNotExistsAsync(context, logger);

                // Create StudentEnquiries table if it doesn't exist
                await CreateStudentEnquiriesTableIfNotExistsAsync(context, logger);

                // Create TeacherEvaluations table if it doesn't exist
                await CreateTeacherEvaluationsTableIfNotExistsAsync(context, logger);

                // Create StudentBankDetails table if it doesn't exist
                await CreateStudentBankDetailsTableIfNotExistsAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ensuring tables are created");
            }
        }

        private static async Task CreateTeachersTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if Teachers table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `Teachers` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("Teachers table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("Teachers table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the Teachers table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `Teachers` (
                            `TeacherId` varchar(10) NOT NULL,
                            `TeacherName` varchar(100) NOT NULL,
                            `Email` varchar(100) NOT NULL,
                            `Password` varchar(100) NOT NULL,
                            `Department` varchar(50) NOT NULL,
                            `RegistrationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            PRIMARY KEY (`TeacherId`),
                            UNIQUE KEY `IX_Teachers_Email` (`Email`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("Teachers table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create Teachers table");
            }
        }

        private static async Task CreateCourseEnrollmentsTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if CourseEnrollments table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `CourseEnrollments` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("CourseEnrollments table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("CourseEnrollments table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the CourseEnrollments table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `CourseEnrollments` (
                            `EnrollmentId` int NOT NULL AUTO_INCREMENT,
                            `StudentId` varchar(10) NOT NULL,
                            `CourseCode` varchar(10) NOT NULL,
                            `CourseName` varchar(100) NOT NULL,
                            `Credits` int NOT NULL,
                            `EnrollmentDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `DropDate` datetime DEFAULT NULL,
                            `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                            PRIMARY KEY (`EnrollmentId`),
                            KEY `IX_CourseEnrollments_StudentId_CourseCode` (`StudentId`,`CourseCode`),
                            CONSTRAINT `FK_CourseEnrollments_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("CourseEnrollments table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create CourseEnrollments table");
            }
        }

        private static async Task CreatePaymentsTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if Payments table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `Payments` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("Payments table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("Payments table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the Payments table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `Payments` (
                            `PaymentId` int NOT NULL AUTO_INCREMENT,
                            `StudentId` varchar(10) NOT NULL,
                            `Amount` decimal(10,2) NOT NULL,
                            `PaymentDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `PaymentMethod` varchar(50) NOT NULL,
                            `PaymentStatus` varchar(20) NOT NULL,
                            `TransactionId` varchar(50) DEFAULT NULL,
                            `Description` varchar(200) DEFAULT NULL,
                            `ReceiptNumber` varchar(50) DEFAULT NULL,
                            PRIMARY KEY (`PaymentId`),
                            KEY `IX_Payments_StudentId` (`StudentId`),
                            CONSTRAINT `FK_Payments_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("Payments table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create Payments table");
            }
        }

        private static async Task CreateStudentEnquiriesTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if StudentEnquiries table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `StudentEnquiries` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("StudentEnquiries table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("StudentEnquiries table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the StudentEnquiries table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `StudentEnquiries` (
                            `EnquiryId` int NOT NULL AUTO_INCREMENT,
                            `StudentId` varchar(10) NOT NULL,
                            `EnquiryType` varchar(20) NOT NULL,
                            `Subject` varchar(100) NOT NULL,
                            `Message` text NOT NULL,
                            `SubmissionDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `Status` varchar(20) NOT NULL DEFAULT 'Pending',
                            `Response` text DEFAULT NULL,
                            `ResponseDate` datetime DEFAULT NULL,
                            `RespondedBy` varchar(100) DEFAULT NULL,
                            PRIMARY KEY (`EnquiryId`),
                            KEY `IX_StudentEnquiries_StudentId` (`StudentId`),
                            CONSTRAINT `FK_StudentEnquiries_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("StudentEnquiries table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create StudentEnquiries table");
            }
        }

        private static async Task CreateStudentsTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if Students table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `Students` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("Students table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("Students table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the Students table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `Students` (
                            `StudentId` varchar(10) NOT NULL,
                            `StudentName` varchar(100) NOT NULL,
                            `PhoneNumber` varchar(20) NOT NULL,
                            `FatherName` varchar(100) NOT NULL,
                            `MotherName` varchar(100) NOT NULL,
                            `Email` varchar(100) NOT NULL,
                            `Password` varchar(100) NOT NULL,
                            `Course` varchar(50) NOT NULL,
                            `RegistrationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `PaymentStatus` tinyint(1) NOT NULL DEFAULT '0',
                            `AmountPaid` decimal(10,2) NOT NULL DEFAULT '0.00',
                            PRIMARY KEY (`StudentId`),
                            UNIQUE KEY `IX_Students_Email` (`Email`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("Students table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create Students table");
            }
        }

        private static async Task CreateCoursesTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if Courses table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `Courses` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("Courses table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("Courses table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the Courses table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `Courses` (
                            `Name` varchar(50) NOT NULL,
                            `Fee` decimal(10,2) NOT NULL,
                            `Description` varchar(200) DEFAULT NULL,
                            PRIMARY KEY (`Name`),
                            UNIQUE KEY `IX_Courses_Name` (`Name`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("Courses table created successfully.");

                    // Seed courses
                    await context.Database.ExecuteSqlRawAsync(@"
                        INSERT IGNORE INTO Courses (Name, Fee, Description)
                        VALUES 
                        ('BTech', 120000, 'Bachelor of Technology'),
                        ('BBA', 80000, 'Bachelor of Business Administration'),
                        ('BCA', 70000, 'Bachelor of Computer Applications'),
                        ('MBA', 150000, 'Master of Business Administration'),
                        ('MCA', 100000, 'Master of Computer Applications');
                    ");
                    logger.LogInformation("Courses seeded successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create Courses table");
            }
        }

        private static async Task CreateTeacherEvaluationsTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if TeacherEvaluations table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `TeacherEvaluations` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("TeacherEvaluations table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("TeacherEvaluations table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the TeacherEvaluations table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `TeacherEvaluations` (
                            `EvaluationId` int NOT NULL AUTO_INCREMENT,
                            `StudentId` varchar(10) NOT NULL,
                            `CourseCode` varchar(10) NOT NULL,
                            `InstructorName` varchar(100) NOT NULL,
                            `TeachingRating` int NOT NULL,
                            `ContentRating` int NOT NULL,
                            `AssessmentRating` int NOT NULL,
                            `CommunicationRating` int NOT NULL,
                            `OverallRating` int NOT NULL,
                            `Comments` text DEFAULT NULL,
                            `SubmissionDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `IsAnonymous` tinyint(1) NOT NULL DEFAULT '1',
                            PRIMARY KEY (`EvaluationId`),
                            KEY `IX_TeacherEvaluations_StudentId` (`StudentId`),
                            CONSTRAINT `FK_TeacherEvaluations_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("TeacherEvaluations table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create TeacherEvaluations table");
            }
        }

        private static async Task CreateStudentBankDetailsTableIfNotExistsAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if StudentBankDetails table exists
                bool tableExists = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `StudentBankDetails` LIMIT 1");
                    tableExists = true;
                    logger.LogInformation("StudentBankDetails table already exists.");
                }
                catch
                {
                    tableExists = false;
                    logger.LogInformation("StudentBankDetails table doesn't exist, creating it now.");
                }

                if (!tableExists)
                {
                    // Create the StudentBankDetails table
                    await context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `StudentBankDetails` (
                            `BankDetailsId` int NOT NULL AUTO_INCREMENT,
                            `StudentId` varchar(10) NOT NULL,
                            `BankName` varchar(100) NOT NULL,
                            `AccountNumber` varchar(20) NOT NULL,
                            `IFSC` varchar(20) NOT NULL,
                            `AccountHolderName` varchar(100) NOT NULL,
                            `LastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            PRIMARY KEY (`BankDetailsId`),
                            UNIQUE KEY `IX_StudentBankDetails_StudentId` (`StudentId`),
                            CONSTRAINT `FK_StudentBankDetails_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`StudentId`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    logger.LogInformation("StudentBankDetails table created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create StudentBankDetails table");
            }
        }
    }
}